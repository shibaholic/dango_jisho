using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EntryRepository : BaseRepository<Entry>, IEntryRepository
{
    private readonly MyDbContext _context;
    public EntryRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<Entry?> GetBy_ent_seq(string ent_seq)
    {
        return await _context.Entries.Where(entry => entry.ent_seq == ent_seq).FirstOrDefaultAsync();
    }
}