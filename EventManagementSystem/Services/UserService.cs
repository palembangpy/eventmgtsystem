using AutoMapper;
using EventManagementSystem.Models.DTO;
using EventManagementSystem.Models.Entities;
using EventManagementSystem.Repository.Interfaces;
using EventManagementSystem.Services.Interfaces;

namespace EventManagementSystem.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepo, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(u => MapToDto(u));
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepo.GetByEmailAsync(email);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        if (await _userRepo.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException($"Email {dto.Email} already exists.");

        var user = _mapper.Map<User>(dto);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.IsActive = true;

        var created = await _userRepo.AddAsync(user);
        _logger.LogInformation("User created: {UserId} - {UserName}", created.Id, created.Name);
        
        return MapToDto(created);
    }

    public async Task UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        _mapper.Map(dto, user);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user);
        _logger.LogInformation("User updated: {UserId}", id);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);
        
        _logger.LogInformation("User soft deleted: {UserId}", id);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByTypeAsync(UserType userType)
    {
        var users = await _userRepo.GetUsersByTypeAsync(userType);
        return users.Select(u => MapToDto(u));
    }

    public async Task<IEnumerable<UserDto>> GetSpeakersAsync()
    {
        var speakers = await _userRepo.GetSpeakersAsync();
        return speakers.Select(u => MapToDto(u));
    }

    public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
    {
        var users = await _userRepo.GetActiveUsersAsync();
        return users.Select(u => MapToDto(u));
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
        => await _userRepo.EmailExistsAsync(email, excludeId);

    public async Task<Dictionary<string, int>> GetUserStatsAsync()
        => await _userRepo.GetUserCountByTypeAsync();

    public async Task<bool?> IsUserActiveAsync(string email)
        => await _userRepo.IsUserActiveAsync(email);


    private UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        UserType = user.UserType.ToString(),
        ProfilePicture = user.ProfilePicture,
        Phone = user.Phone,
        Address = user.Address,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        TotalCertificates = user.Certificates?.Count ?? 0,
        TotalEvents = user.EventsAsSpeaker?.Count ?? 0
    };
}
