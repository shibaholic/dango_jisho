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
        var queryable = _context.Entries.AsQueryable();
        
        queryable = queryable.Where(q => q.KanjiElements.Any(k => k.keb.Contains(query)) || 
                                         q.ReadingElements.Any(r => r.reb.Contains(query)) ||
                                         q.Senses.Any(s => s.gloss.FirstOrDefault(g => g.Contains(query)) != null)
                                         );

        queryable = queryable
            .Include(q => q.KanjiElements)
            .Include(q => q.ReadingElements)
            .Include(q => q.Senses)
            .ThenInclude(s => s.lsource);

        var entries = queryable.ToList();
        
        var orderedEntries = entries.OrderBy(e => e.KanjiElements.Min(k => k.keb.Length))
            .ThenBy(e => e.ReadingElements.Min(r => r.reb.Length));
        
        return orderedEntries.ToList();
    }
}