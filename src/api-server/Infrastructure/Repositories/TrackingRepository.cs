using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;

namespace Infrastructure.Repositories;

public class TrackingRepository: BaseRepository<EntryIsTagged>, ITrackingRepository
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
    
}