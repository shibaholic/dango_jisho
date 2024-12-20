using System.Data;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories;

public class EntryRepository : BaseRepository<Entry>, IEntryRepository
{
    private readonly MyDbContext _context;
    public EntryRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task BulkInsertAsync(List<Entry> entries)
    {
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        // import the parent entity Entry
        using (var writer = connection.BeginBinaryImport($"COPY \"Entries\" (\"ent_seq\") FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var entry in entries)
            {
                writer.StartRow();
                
                writer.Write(entry.ent_seq, NpgsqlTypes.NpgsqlDbType.Text);
            }
            
            await writer.CompleteAsync();
        }
        
        // import the child entity KanjiElement
        // flattens resulting sequences into one sequence
        var kanjiElements = entries.SelectMany(e => e.KanjiElements).ToList();
        for (var i = 0; i < kanjiElements.Count(); i++)
        {
            kanjiElements[i].Id = i;
        }
        using (var writer = connection.BeginBinaryImport(
                   $"COPY \"KanjiElements\" (\"Id\", \"ent_seq\", \"keb\", \"ke_inf\", \"ke_pri\") FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var k_ele in kanjiElements)
            {
                writer.StartRow();
                
                writer.Write(k_ele.Id, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(k_ele.ent_seq, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(k_ele.keb, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(k_ele.ke_inf, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(k_ele.ke_pri, NpgsqlTypes.NpgsqlDbType.Text);
            }
            
            await writer.CompleteAsync();
        }
        
        // import the child entity ReadingElement
        var readingElements = entries.SelectMany(e => e.ReadingElements).ToList();
        // for (var i = 0; i < readingElements.Count(); i++)
        // {
        //     readingElements[i].Id = i;
        // }
        using (var writer = connection.BeginBinaryImport(
                   $"COPY \"ReadingElements\" (\"Id\", \"ent_seq\", \"reb\", \"re_nokanji\", \"re_restr\", \"re_inf\", \"re_pri\") FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var r_ele in readingElements)
            {
                writer.StartRow();
                
                writer.Write(r_ele.Id, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(r_ele.ent_seq, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(r_ele.reb, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(r_ele.re_nokanji, NpgsqlTypes.NpgsqlDbType.Boolean);
                writer.Write(r_ele.re_restr, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(r_ele.re_inf, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(r_ele.re_pri, NpgsqlTypes.NpgsqlDbType.Text);
            }
            
            await writer.CompleteAsync();
        }
        
        // import the child entity Sense
        var senses = entries.SelectMany(e => e.Senses).ToList();
        // for (var i = 0; i < senses.Count(); i++)
        // {
        //     senses[i].Id = i;
        // }
        using (var writer = connection.BeginBinaryImport(
                   $"COPY \"Senses\" (\"Id\", \"ent_seq\", \"stagk\", \"stagr\", \"pos\", " +
                   $"\"xref\", \"ant\", \"field\", \"misc\", \"s_inf\", \"dial\", \"gloss\") FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var sense in senses)
            {
                writer.StartRow();
                
                writer.Write(sense.Id, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(sense.ent_seq, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.stagk, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.stagr, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.pos, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.xref, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.ant, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.field, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.misc, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.s_inf, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.dial, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(sense.gloss, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text);
            }
            
            await writer.CompleteAsync();
        }
        
        // import the child child entity LSource
        var lsources = entries.SelectMany(e => e.Senses.SelectMany(s => s.lsource)).ToList();
        // for (var i = 0; i < lsources.Count(); i++)
        // {
        //     lsources[i].Id = i;
        // }
        using (var writer = connection.BeginBinaryImport(
                   $"COPY \"LSource\" (\"Id\", \"SenseId\", \"LangValue\", \"lang\", \"ls_part\", \"ls_wasei\") FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var lsource in lsources)
            {
                writer.StartRow();
                
                writer.Write(lsource.Id, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(lsource.SenseId, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(lsource.LangValue, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(lsource.lang, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(lsource.ls_part, NpgsqlTypes.NpgsqlDbType.Boolean);
                writer.Write(lsource.ls_wasei, NpgsqlTypes.NpgsqlDbType.Boolean);
            }
            
            await writer.CompleteAsync();
        }
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