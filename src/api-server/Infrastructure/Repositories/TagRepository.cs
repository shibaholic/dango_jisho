using Application;
using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagRepository : BaseRepository<Tag>, ITagRepository
{
    private readonly MyDbContext _context;
    public TagRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Tag?> ReadByIdUserIdAsync(Guid id, Guid userId)
    {
        var query = _context.Tags.Where(t => t.Id == id && t.UserId == userId);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<EntryIsTagged> CreateEntryIsTaggedAsync(EntryIsTagged entryIsTagged)
    {
        await _context.EntryIsTagged.AddAsync(entryIsTagged);

        return entryIsTagged;
    }
}