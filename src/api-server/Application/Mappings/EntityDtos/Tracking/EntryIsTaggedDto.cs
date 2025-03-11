using Domain.Entities.Tracking;

namespace Application.Mappings.EntityDtos.Tracking;

public class EIT_TEDto
{
    // public string ent_seq { get; set; } 
    // public Guid TagId { get; set; }
    public DateTime AddedToTagDate { get; set; }
    public int UserOrder { get; set; }
    public TE_EntryDto TrackedEntry { get; set; }
}

public class EIT_TagDto
{
    public DateTime AddedToTagDate { get; set; }
    public int UserOrder { get; set; }
    public TagDto Tag { get; set; }
}