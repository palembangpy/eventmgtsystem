using EventManagementSystem.Models.Entities;

namespace EventManagementSystem.Repository.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<IEnumerable<User>> GetUsersByTypeAsync(UserType userType);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetSpeakersAsync();
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<Dictionary<string, int>> GetUserCountByTypeAsync();
}