using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MyDbContext _context;

    public UnitOfWork(MyDbContext context)
    {
        _context = context;
    }

    public async Task<int> Commit(CancellationToken cancellationToken=default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DetachAllEntries(CancellationToken cancellationToken=default)
    {
        var entries_count = _context.ChangeTracker.Entries().Count();
        foreach (var entry in _context.ChangeTracker.Entries().ToList())
        {
            entry.State = EntityState.Detached;
        }

        return entries_count;
    }
}