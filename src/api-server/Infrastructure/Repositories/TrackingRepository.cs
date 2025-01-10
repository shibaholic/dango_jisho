using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

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

    public async Task<TrackedEntry> CreateTrackedEntryAsync(TrackedEntry trackedEntry)
    {
        await _context.TrackedEntries.AddAsync(trackedEntry);
        
        return trackedEntry;
    }

    public async Task<TrackedEntry?> ReadTrackedEntryByIdsAsync(string ent_seq, Guid userId)
    {
        return await _context.TrackedEntries
            .Where(te => te.ent_seq == ent_seq && te.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<EntryEvent> CreateReviewEventAsync(EntryEvent entryEvent)
    {
        await _context.EntryEvents.AddAsync(entryEvent);
        
        return entryEvent;
    }
}