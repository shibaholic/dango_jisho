using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IBaseRepository<T> where T : IBaseEntity
{
    Task<T> ReadByIdAsync(Guid id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<T> CreateAsync(T entity);
    Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}