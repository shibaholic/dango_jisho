using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IBaseRepository<T> where T : IBaseEntity
{
    Task<T?> ReadByIdAsync(object id);
    Task<IEnumerable<T>> ReadAllAsync(int? take = null);
    Task<T> CreateAsync(T entity);
    Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}