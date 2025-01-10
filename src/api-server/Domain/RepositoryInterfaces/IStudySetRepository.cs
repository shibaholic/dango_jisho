using Domain.Entities.Tracking;

namespace Domain.RepositoryInterfaces;

public interface IStudySetRepository : IBaseRepository<StudySet>
{
    Task<StudySet?> ReadByIdUserId(Guid id, Guid userId);
    Task<List<TrackedEntry>> GetTrackedEntriesByStudySet(Guid studySetId);
}