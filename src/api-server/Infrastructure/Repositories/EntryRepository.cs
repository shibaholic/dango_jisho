using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using Domain.Entities;
using Domain.Entities.JMDict;
using Domain.Enums;
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
                writer.Write(EnumExtension.ToDatabaseValue(k_ele.ke_pri), NpgsqlTypes.NpgsqlDbType.Text);
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
                writer.Write(EnumExtension.ToDatabaseValue(r_ele.re_pri), NpgsqlTypes.NpgsqlDbType.Text);
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
        return await _context.Entries.Where(entry => entry.ent_seq == ent_seq)
            .Include(entry => entry.KanjiElements)
            .Include(entry => entry.ReadingElements)
            .Include(entry => entry.Senses)
            .ThenInclude(sense => sense.lsource)
            .FirstOrDefaultAsync();
    }
    public async Task<List<Entry>> Search(string query)
    {
        var pageSize = 10;  // Number of entries per page
        var pageNumber = 1; // Current page (1-based index)
        
        // detect what type of query it is.
        // if it contains kanji then search in the kanji elements
        // next if it contains hiragana/katakana then search reading elements
        // finally search gloss
        Expression<Func<Entry, bool>> whereExpr;
        Func<Entry, bool> exactMatchExpr;
        if (Regex.IsMatch(query, @"[\u4E00-\u9FAF]"))
        {
            Console.WriteLine("Searching kanji");
            whereExpr = q => q.KanjiElements.Any(k => k.keb.Contains(query));
            exactMatchExpr = e => e.KanjiElements.Any(k => k.keb == query);
        }
        else if (Regex.IsMatch(query, @"[\u3040-\u309F\u30A0-\u30FF]"))
        {
            Console.WriteLine("Searching kana");
            whereExpr = q => q.ReadingElements.Any(r => r.reb.Contains(query));
            exactMatchExpr = e => e.ReadingElements.Any(r => r.reb == query);
        }
        else
        {
            Console.WriteLine("Searching gloss");
            whereExpr = q => q.Senses.Any(s => s.gloss.Any(g => g.Contains(query)));
            exactMatchExpr = e => e.Senses.Any(s => s.gloss.Any(g => g == query));
        }
        
        // Start with base queryable
        var queryable = _context.Entries
            .Where(whereExpr)
            .Include(q => q.KanjiElements)
            .Include(q => q.ReadingElements)
            .Include(q => q.Senses)
            .ThenInclude(s => s.lsource);
        
        // Apply ordering logic
        var orderedQueryable = queryable
            .OrderBy(e => e.KanjiElements.Any() ? e.KanjiElements.Min(k => k.keb.Length) : int.MaxValue)
            .ThenBy(e => e.ReadingElements.Any() ? e.ReadingElements.Min(r => r.reb.Length) : int.MaxValue);
        
        // Apply pagination
        var list = await orderedQueryable
            // .Skip((pageNumber - 1) * pageSize)
            // .Take(pageSize)
            .ToListAsync();
        
        // find exact matches and put them first in the order
        var orderedList = list.OrderByDescending(exactMatchExpr)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return orderedList;
    }
}