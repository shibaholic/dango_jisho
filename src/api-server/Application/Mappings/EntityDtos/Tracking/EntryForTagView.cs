using Application.Mappings.EntityDtos.JMDict;

namespace Application.Mappings.EntityDtos.Tracking;

public class EntryForTagViewDto
{
    public string FaceTerm { get; set; }
    public string? Reading { get; set; }
    public List<SenseDto> Senses { get; set; }  
    public DateTime AddedToTagDate { get; set; }
    public int UserOrder { get; set; }
}