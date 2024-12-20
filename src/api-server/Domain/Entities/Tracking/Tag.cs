namespace Domain.Entities.Tracking;

public class Tag : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // foreign key
    public string Name { get; set; }
    public DateTimeOffset Created { get; set; }
    public List<EntryIsTagged> EntryIsTaggeds { get; set; } =  new List<EntryIsTagged>();
    
    public User User { get; set; } // nav
}