using Microsoft.EntityFrameworkCore;
using WorkingMVC.Data.Entities;
using WorkingMVC.Interfaces;

namespace WorkingMVC.Repositories;

public abstract class BaseRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id));
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet
            .Where(x => x.IsDeleted == false)
            .ToListAsync();
    }

    public virtual Task<IQueryable<TEntity>> GetAllQueryableAsync()
    {
        IQueryable<TEntity> query = _dbSet.Where(x => x.IsDeleted == false);
        return Task.FromResult(query);
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    // Hard delete — optional: can convert to soft delete
    public virtual async Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}