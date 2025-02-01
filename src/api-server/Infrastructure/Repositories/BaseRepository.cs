using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : IBaseEntity
{
    private readonly MyDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(MyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> ReadByIdAsync(object id)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));

        var keyProperty = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault();
        if (keyProperty == null)
        {
            throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key.");
        }

        var keyType = keyProperty.ClrType;

        if (id.GetType() != keyType)
        {
            // Attempt conversion to the expected type
            id = Convert.ChangeType(id, keyType);
        }
        
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int? take = null)
    {
        if (take is not null)
        {
            return await _dbSet.Take(take.Value).ToListAsync();
        }
        return await _dbSet.ToListAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        ValidateEntities();
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities)
    {
        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await ReadByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    
    private void ValidateEntities()
    {
        var changeTracker = _context.ChangeTracker;
        var entries = changeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(entity, context, results, true))
            {
                var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Entity validation failed: {errors}");
            }
        }
    }
}