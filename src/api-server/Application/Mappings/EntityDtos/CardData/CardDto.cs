using Application.Mappings.EntityDtos.JMDict;

namespace Application.Mappings.EntityDtos.CardData;

public record CardDto
{
    public string ent_seq { get; set; }
    public Guid? UserId { get; set; }
    public KanjiElementDto KanjiElement { get; set; }
    public ReadingElementDto ReadingElement { get; set; }
    public List<SenseDto> Senses { get; set; }
}