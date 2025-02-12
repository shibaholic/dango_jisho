using Domain.Entities.CardData;

namespace Domain.RepositoryInterfaces;

public interface ICardRepository : IBaseRepository<Card>
{
    Task BulkInsertAsync(List<Card> cards);
    Task BulkInsertCardSenseAsync(List<CardSense> cardSenses);
    Task<Card?> ReadByEntSeq(string ent_seq);
    Task<List<Card>> Search(string query);
}