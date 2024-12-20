using Domain.Entities.JMDict;

namespace Domain.Entities.Tracking;

public class EntryIsTagged
{
    public string ent_seq { get; set; } // foreign key composite
    public Guid TagId { get; set; }     // foreign key composite
    public DateTimeOffset Created { get; set; }
    public int UserOrder { get; set; }
    
    public Entry Entry { get; set; } // parent nav
    public Tag Tag { get; set; }     // parent nav
    
    public TrackedEntry TrackedEntry { get; set; } // child nav
}