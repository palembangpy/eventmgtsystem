using EventManagementSystem.Models.DTO;
using EventManagementSystem.Models.Entities;

namespace EventManagementSystem.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task DeleteUserAsync(Guid id);
    Task<IEnumerable<UserDto>> GetUsersByTypeAsync(UserType userType);
    Task<IEnumerable<UserDto>> GetSpeakersAsync();
    Task<IEnumerable<UserDto>> GetActiveUsersAsync();
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<Dictionary<string, int>> GetUserStatsAsync();
}
