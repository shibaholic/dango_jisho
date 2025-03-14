using Domain.Entities.Tracking;
using Domain.RepositoryInterfaces.ReturnTypes;

namespace Domain.RepositoryInterfaces;

public interface ITrackingRepository : IBaseRepository<TrackedEntry>
{
    Task<EntryIsTagged> CreateEntryIsTaggedAsync(EntryIsTagged entryIsTagged);
    Task<TrackedEntry?> ReadTrackedEntryByIdsAsync(string ent_seq, Guid userId);
    Task<PagedResult<List<TrackedEntry>>> ReadTrackedEntryByTagIdAsync(Guid tagId, Guid userId, int pageIndex, int pageSize);
    Task<EntryEvent> CreateReviewEventAsync(EntryEvent entryEvent);
    Task<TrackedEntry?> ReadNextReview(Guid userId, Guid tagId);
}