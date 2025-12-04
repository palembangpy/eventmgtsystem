namespace EventManagementSystem.Repository;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();
    public virtual async Task<T?> GetByIdAsync(int id)
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
    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    public virtual async Task<bool> ExistsAsync(int id)
        => await GetByIdAsync(id) != null;
    public virtual async Task<int> CountAsync()
        => await _dbSet.CountAsync();
    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
