using EventManagementSystem.Core.Models.Entities;

namespace EventManagementSystem.Core.Interfaces.Repository;

public interface IUserRepository : IBaseRepository<User>
{
    Task<IEnumerable<User>> GetUsersByTypeAsync(UserType userType);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetSpeakersAsync();
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<Dictionary<string, int>> GetUserCountByTypeAsync();
    Task<bool?> IsUserActiveAsync(string email);
}