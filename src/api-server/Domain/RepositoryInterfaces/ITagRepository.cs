using Domain.Entities.Tracking;

namespace Domain.RepositoryInterfaces;

public interface ITagRepository : IBaseRepository<Tag>
{
    Task<Tag?> ReadByIdUserIdAsync(Guid id, Guid userId);
    Task<List<Tag>> ReadAllByUserIdAsync(Guid userId);
}