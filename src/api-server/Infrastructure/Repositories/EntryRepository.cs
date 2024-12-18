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
    public async Task<List<Entry>> Search(string query)
    {
        // Start by checking for Where on KanjiElement
        var k_query = _context.Set<KanjiElement>().AsQueryable();
        
        k_query = k_query.Where(k => k.keb.Contains(query));

        var join_query = k_query.Join(_context.Entries,
            k => k.ent_seq,
            e => e.ent_seq,
            (k, e) => new { k, e });
        
        var e_query = join_query.Select(join => join.e)
            .Include(join => join.KanjiElements)
            .Include(join => join.ReadingElements)
            .Include(join => join.Senses)
            .ThenInclude(s => s.lsource);
        
        return e_query.ToList();
    }
}