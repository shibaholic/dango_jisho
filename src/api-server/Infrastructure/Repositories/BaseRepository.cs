using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;

namespace Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : IBaseEntity
{
    private readonly MyDbContext _context;

    public BaseRepository(MyDbContext context)
    {
        _context = context;
    }

    public void Create(T entity)
    {
        _context.Add(entity);
    }

    public async Task RangeCreate(List<T> entities)
    {
        await _context.AddRangeAsync(entities);
    }

    public void Delete(T entity)
    {
        _context.Remove(entity);
    }

    public List<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public void Update(T entity)
    {
        _context.Update(entity);
    }
}