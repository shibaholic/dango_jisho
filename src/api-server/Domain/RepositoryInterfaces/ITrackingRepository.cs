using Domain.Entities.Tracking;

namespace Domain.RepositoryInterfaces;

public interface ITrackingRepository : IBaseRepository<EntryIsTagged>
{
    Task<EntryIsTagged> CreateEntryIsTaggedAsync(EntryIsTagged entryIsTagged);
    Task<TrackedEntry> CreateTrackedEntryAsync(TrackedEntry trackedEntry);
    Task<TrackedEntry?> ReadTrackedEntryByIdsAsync(string ent_seq, Guid userId);
    Task<ReviewEvent> CreateReviewEventAsync(ReviewEvent reviewEvent);
}