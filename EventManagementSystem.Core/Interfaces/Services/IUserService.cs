using EventManagementSystem.Core.DTO;
using EventManagementSystem.Core.Models.Entities;
namespace EventManagementSystem.Core.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task DeleteUserAsync(Guid id);
    Task<IEnumerable<UserDto>> GetUsersByTypeAsync(UserType userType);
    Task<IEnumerable<UserDto>> GetSpeakersAsync();
    Task<IEnumerable<UserDto>> GetActiveUsersAsync();
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<Dictionary<string, int>> GetUserStatsAsync();
    Task<bool?> IsUserActiveAsync(string email);
}
