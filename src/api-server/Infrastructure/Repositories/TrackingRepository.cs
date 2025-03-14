using AutoMapper;
using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using Domain.RepositoryInterfaces.ReturnTypes;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Infrastructure.Repositories;

public class TrackingRepository: BaseRepository<TrackedEntry>, ITrackingRepository
{
    private readonly MyDbContext _context;
    public TrackingRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<EntryIsTagged> CreateEntryIsTaggedAsync(EntryIsTagged entryIsTagged)
    {
        await _context.EntryIsTagged.AddAsync(entryIsTagged);

        return entryIsTagged;
    }
    
    public async Task<TrackedEntry?> ReadTrackedEntryByIdsAsync(string ent_seq, Guid userId)
    {
        return await _context.TrackedEntries
            .Where(te => te.ent_seq == ent_seq && te.UserId == userId)
            .Include(te => te.Entry)
            .ThenInclude(e => e.KanjiElements)
            .Include(te => te.Entry)
            .ThenInclude(e => e.ReadingElements)
            .Include(te => te.Entry)
            .ThenInclude(e => e.Senses)
            .Include(te => te.Entry)
            .ThenInclude(e => e.Senses)
            .ThenInclude(s => s.lsource)
            .Include(te => te.EntryIsTaggeds)
            .ThenInclude(eit => eit.Tag)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResult<List<TrackedEntry>>> ReadTrackedEntryByTagIdAsync(Guid tagId, Guid userId, int pageIndex, int pageSize)
    {
        var query = _context.TrackedEntries
            .Join(_context.EntryIsTagged, te => te.ent_seq, eit => eit.ent_seq, (te, eit) => new { te, eit })
            .Where(join => join.eit.TagId == tagId && join.te.UserId == userId)
            .Select(join => join.te)
            .Include(te => te.Entry)
            .Include(te => te.EntryIsTaggeds)
            .ThenInclude(eit => eit.Tag);
        
        var totalEntities = await query.CountAsync();

        var result = await query.Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pagedResult = new PagedResult<List<TrackedEntry>>
        {
            Data = result,
            TotalElements = totalEntities,
        };

        return pagedResult;
    }

    public async Task<EntryEvent> CreateReviewEventAsync(EntryEvent entryEvent)
    {
        await _context.EntryEvents.AddAsync(entryEvent);
        
        return entryEvent;
    }

    public async Task<TrackedEntry?> ReadNextReview(Guid userId, Guid tagId)
    {
        var now = DateTime.UtcNow;
        
        var query = _context.TrackedEntries
            .Where(te => te.UserId == userId && (te.LastReviewDate == null || te.SpacedTime == null || te.LastReviewDate.Value + te.SpacedTime.Value <= now))
            .OrderBy(te => te.LevelStateType == LevelStateType.New ? 0 : 1)
            .ThenBy(te => te.SpacedTime)
            .Include(te => te.Entry)
            .ThenInclude(e => e.KanjiElements)
            .Include(te => te.Entry)
            .ThenInclude(e => e.ReadingElements)
            .Include(te => te.Entry)
            .ThenInclude(e => e.Senses)
            .Include(te => te.Entry)
            .ThenInclude(e => e.Senses)
            .ThenInclude(s => s.lsource)
            .Include(te => te.EntryIsTaggeds);

        return await query.FirstOrDefaultAsync();
    }
}