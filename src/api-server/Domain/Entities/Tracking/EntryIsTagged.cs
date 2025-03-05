using Domain.Entities.JMDict;

namespace Domain.Entities.Tracking;

public class EntryIsTagged : IBaseEntity
{
    public string ent_seq { get; set; } // foreign key composite
    public Guid UserId { get; set; } // foreign key composite
    public Guid TagId { get; set; }     // foreign key
    public DateTime AddedToTagDate { get; set; }
    public int UserOrder { get; set; }
    
    // public Entry Entry { get; set; } // parent nav
    public TrackedEntry TrackedEntry { get; set; }
    public Tag Tag { get; set; }     // parent nav

    public EntryIsTagged()
    {
    }
    
    public EntryIsTagged(Tag tag, Entry entry) {}
}