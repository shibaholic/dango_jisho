using System.Data;
using Domain.Entities.CardData;
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
}