using Domain.Entities.Tracking;

namespace Application.Mappings.EntityDtos.Tracking;

public class EntryIsTaggedDto
{
    // public string ent_seq { get; set; } 
    // public Guid TagId { get; set; }
    public DateTime AddedToTagDate { get; set; }
    public int UserOrder { get; set; }
    public TrackedEntryDto? TrackedEntry { get; set; }
}