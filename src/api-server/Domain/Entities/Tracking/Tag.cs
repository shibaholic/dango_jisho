using System.ComponentModel.DataAnnotations;
using Domain.Entities.JMDict;

namespace Domain.Entities.Tracking;

public class Tag : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // foreign key
    [Required(ErrorMessage = "The string cannot be null or empty.")]
    [MinLength(1, ErrorMessage = "The string must not be empty.")]
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public int TotalEntries { get; set; } = 0;
    public int TotalKnown { get; set; } = 0;
    public int TotalReviewing { get; set; } = 0;
    public int TotalLearning { get; set; } = 0;
    public int TotalNew { get; set; } = 0;
    
    public User User { get; set; } // nav
    public List<EntryIsTagged> EntryIsTaggeds { get; set; } =  new List<EntryIsTagged>();
    public List<TagInStudySet> TagInStudySets { get; set; } =  new List<TagInStudySet>();
}