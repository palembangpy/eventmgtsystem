using EventManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Core.Interfaces.Repository;
namespace EventManagementSystem.Infrastructure.Repository;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();
    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    public virtual async Task<bool> ExistsAsync(Guid id)
        => await GetByIdAsync(id) != null;
    public virtual async Task<int> CountAsync()
        => await _dbSet.CountAsync();
    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
