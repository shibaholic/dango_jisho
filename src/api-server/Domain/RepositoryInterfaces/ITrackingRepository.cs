using Domain.Entities.Tracking;

namespace Domain.RepositoryInterfaces;

public interface ITrackingRepository : IBaseRepository<EntryIsTagged>
{
    Task AddEntryToTag(string ent_seq, Guid TagId, Guid UserId);
}