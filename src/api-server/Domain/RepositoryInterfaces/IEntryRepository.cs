using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IEntryRepository : IBaseRepository<Entry>
{
    public Task BulkInsertAsync(List<Entry> entries);
    public Task<Entry?> GetBy_ent_seq(string ent_seq);
    public Task<List<Entry>> Search(string query);
}