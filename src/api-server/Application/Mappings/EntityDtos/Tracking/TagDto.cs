using Domain.Entities.Tracking;

namespace Application.Mappings.EntityDtos.Tracking;

public class TagDto
{
    public Guid Id { get; set; }
    // public Guid UserId { get; set; } // foreign key
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public int TotalEntries { get; set; }
    public int TotalKnown { get; set; }
    public int TotalReviewing { get; set; }
    public int TotalLearning { get; set; }
    public int TotalNew { get; set; }
    public List<EntryIsTaggedDto> EntryIsTaggeds { get; set; } = null;
}