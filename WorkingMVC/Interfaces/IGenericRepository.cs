using WorkingMVC.Data.Entities;

namespace WorkingMVC.Interfaces;

public interface IGenericRepository<TEntity, TKey> where TEntity : IEntity<TKey>
{
    Task<TEntity> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IQueryable<TEntity>> GetAllQueryableAsync();
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}