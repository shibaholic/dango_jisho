using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Domain.Entities.CardData;
using Domain.Entities.JMDict;
using Domain.RepositoryInterfaces;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Repositories;

public class CardRepository : BaseRepository<Card>, ICardRepository
{
    private readonly MyDbContext _context;
    public CardRepository(MyDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task BulkInsertAsync(List<Card> cards)
    {
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        
        using (var writer = connection.BeginBinaryImport("COPY \"Cards\" (\"Id\", \"ent_seq\", \"UserId\", \"KanjiId\", \"ReadingId\") FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var card in cards)
            {
                writer.StartRow();
                
                writer.Write(card.Id, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(card.ent_seq, NpgsqlTypes.NpgsqlDbType.Text);
                writer.Write(card.UserId, NpgsqlTypes.NpgsqlDbType.Uuid);
                writer.Write(card.KanjiId, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(card.ReadingId, NpgsqlTypes.NpgsqlDbType.Integer);
            }
            
            await writer.CompleteAsync();
        }
    }

    public async Task BulkInsertCardSenseAsync(List<CardSense> cardSenses)
    {
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        
        using (var writer = connection.BeginBinaryImport("COPY \"CardSenses\" (\"CardId\", \"SenseId\") FROM STDIN (FORMAT BINARY)"))
        {
            foreach (var cardSense in cardSenses)
            {
                writer.StartRow();
                
                writer.Write(cardSense.CardId, NpgsqlTypes.NpgsqlDbType.Integer);
                writer.Write(cardSense.SenseId, NpgsqlTypes.NpgsqlDbType.Integer);
            }
            
            await writer.CompleteAsync();
        }
    }
    
    public async Task<Card?> ReadByEntSeq(string ent_seq)
    {
        return await _context.Cards.Where(c => c.ent_seq == ent_seq)
            .Include(c => c.KanjiElement)
            .Include(c => c.ReadingElement)
            .Include(c => c.Senses)
            .ThenInclude(sense => sense.lsource)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Card>> Search(string query)
    {
        throw new NotImplementedException();

        // var pageSize = 10;  // Number of entries per page
        // var pageNumber = 1; // Current page (1-based index)
        //
        // Expression<Func<Entry, bool>> whereExpr;
        // Func<Entry, bool> exactMatchExpr;
        // if (Regex.IsMatch(query, @"[\u4E00-\u9FAF]"))
        // {
        //     Console.WriteLine("Searching kanji");
        //     whereExpr = q => q.KanjiElements.Any(k => k.keb.Contains(query));
        //     exactMatchExpr = e => e.KanjiElements.Any(k => k.keb == query);
        // }
        // else if (Regex.IsMatch(query, @"[\u3040-\u309F\u30A0-\u30FF]"))
        // {
        //     Console.WriteLine("Searching kana");
        //     whereExpr = q => q.ReadingElements.Any(r => r.reb.Contains(query));
        //     exactMatchExpr = e => e.ReadingElements.Any(r => r.reb == query);
        // }
        // else
        // {
        //     Console.WriteLine("Searching gloss");
        //     whereExpr = q => q.Senses.Any(s => s.gloss.Any(g => g.Contains(query)));
        //     exactMatchExpr = e => e.Senses.Any(s => s.gloss.Any(g => g == query));
        // }
        //
        // var queryable = _context.Entries
        //     .Where(whereExpr)
        //     .Join(_context.Cards,
        //         e => e.ent_seq,
        //         c => c.ent_seq,
        //         (e, c) => c )
        //     .Include(c => c.KanjiElement)
        //     .Include(c => c.ReadingElement)
        //     .Include(c => c.Senses)
        //     .ThenInclude(s => s.lsource);
        //
        // var list = await queryable.ToListAsync();
        //
        // var orderedList = list.OrderByDescending(exactMatchExpr)
        //     .Skip((pageNumber - 1) * pageSize)
        //     .Take(pageSize)
        //     .ToList();
    }
}