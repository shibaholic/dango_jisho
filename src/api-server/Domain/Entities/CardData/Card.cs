using Domain.Entities.JMDict;

namespace Domain.Entities.CardData;

public class Card : IBaseEntity
{
    public int Id { get; set; }
    public string ent_seq { get; set; }
    public Guid? UserId { get; set; }
    public int? KanjiId { get; set; } = null;
    public KanjiElement? KanjiElement { get; set; }
    public int ReadingId { get; set; }
    public ReadingElement ReadingElement { get; set; }
    // public List<int> SenseIds { get; set; }
    public List<Sense> Senses { get; set; } = new();
    public List<CardSense> CardSenses { get; set; } = new();
}