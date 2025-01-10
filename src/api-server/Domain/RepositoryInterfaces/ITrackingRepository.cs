using Domain.Entities.Tracking;

namespace Domain.RepositoryInterfaces;

public interface ITrackingRepository : IBaseRepository<TrackedEntry>
{
    Task<EntryIsTagged> CreateEntryIsTaggedAsync(EntryIsTagged entryIsTagged);
    Task<TrackedEntry> CreateTrackedEntryAsync(TrackedEntry trackedEntry);
    Task<TrackedEntry?> ReadTrackedEntryByIdsAsync(string ent_seq, Guid userId);
    Task<EntryEvent> CreateReviewEventAsync(EntryEvent entryEvent);
}