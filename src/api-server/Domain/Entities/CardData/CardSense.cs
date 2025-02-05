using Domain.Entities.JMDict;

namespace Domain.Entities.CardData;

public class CardSense
{
    public int CardId { get; set; }
    public int SenseId { get; set; }
    public Card Card { get; set; }
    public Sense Sense { get; set; }
}