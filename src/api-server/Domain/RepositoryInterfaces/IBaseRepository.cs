using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IBaseRepository<T> where T : IBaseEntity
{
    void Create(T entity);
    // Task BulkCreate(List<T> entities);
    Task RangeCreate(List<T> entities);
    void Update(T entity);
    void Delete(T entity);
    List<T> GetAll();
}