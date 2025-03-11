using System.Text;
using Application.Mappings.EntityDtos.Tracking;

namespace Application.Mappings.EntityDtos.JMDict;

public record Entry_TEDto
{
    public string ent_seq { get; set; }
    public int? SelectedKanjiIndex { get; set; } = null;
    public int SelectedReadingIndex { get; set; }
    public int? PriorityScore { get; set; } = null;
    public List<KanjiElementDto> KanjiElements { get; set; }
    public List<ReadingElementDto> ReadingElements { get; set; }
    public List<SenseDto> Senses { get; set; }
    public TE_EITDto TrackedEntry { get; set; }
    
    public Entry_TEDto()
    {
        KanjiElements = new List<KanjiElementDto>();
        ReadingElements = new List<ReadingElementDto>();
        Senses = new List<SenseDto>();
    }
}

public record EntryDto
{
    public string ent_seq { get; set; }
    public int? SelectedKanjiIndex { get; set; } = null;
    public int SelectedReadingIndex { get; set; }
    public int? PriorityScore { get; set; } = null;
    public List<KanjiElementDto> KanjiElements { get; set; }
    public List<ReadingElementDto> ReadingElements { get; set; }
    public List<SenseDto> Senses { get; set; }
    
    public EntryDto()
    {
        KanjiElements = new List<KanjiElementDto>();
        ReadingElements = new List<ReadingElementDto>();
        Senses = new List<SenseDto>();
    }
}