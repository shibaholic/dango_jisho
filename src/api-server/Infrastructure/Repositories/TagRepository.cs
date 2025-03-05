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

    public async Task<List<Tag>> ReadAllByUserIdAsync(Guid userId)
    {
        var query = _context.Tags.Where(t => t.UserId == userId)
            .Include(t => t.EntryIsTaggeds)
            .ThenInclude(eit => eit.TrackedEntry)
            .ThenInclude(te => te.Entry)
            .ThenInclude(entry => entry.KanjiElements)
            .Include(t => t.EntryIsTaggeds)
            .ThenInclude(eit => eit.TrackedEntry)
            .ThenInclude(te => te.Entry)
            .ThenInclude(entry => entry.ReadingElements)
            .Include(t => t.EntryIsTaggeds)
            .ThenInclude(eit => eit.TrackedEntry)
            .ThenInclude(te => te.Entry)
            .ThenInclude(e => e.Senses);
        
        return await query.ToListAsync();
    }
}