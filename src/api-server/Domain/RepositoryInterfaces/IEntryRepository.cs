using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IEntryRepository : IBaseRepository<Entry>
{
    public Task<Entry?> GetBy_ent_seq(string ent_seq);
}