using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Tracking;

public class Tag : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // foreign key
    [Required(ErrorMessage = "The string cannot be null or empty.")]
    [MinLength(1, ErrorMessage = "The string must not be empty.")]
    public string Name { get; set; }
    public DateTimeOffset Created { get; set; }
    public int TotalEntries { get; set; } = 0;
    public List<EntryIsTagged> EntryIsTaggeds { get; set; } =  new List<EntryIsTagged>();
    
    public User User { get; set; } // nav
}