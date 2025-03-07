using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Domain.Entities.JMDict;

namespace Domain.Entities.Tracking;

public class Tag : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // foreign key
    [Required(ErrorMessage = "The string cannot be null or empty.")]
    [MinLength(1, ErrorMessage = "The string must not be empty.")]
    public string Name { get; set; }
    public DateTime DateCreated { get; set; }
    public int TotalEntries { get; set; } = 0;
    public int TotalKnown { get; set; } = 0;
    public int TotalReviewing { get; set; } = 0;
    public int TotalLearning { get; set; } = 0;
    public int TotalNew { get; set; } = 0;
    
    public User User { get; set; } // nav
    private List<EntryIsTagged> _entryIsTaggeds =  new();
    public IReadOnlyList<EntryIsTagged> EntryIsTaggeds => _entryIsTaggeds.AsReadOnly();
    private List<TagInStudySet> _tagInStudySets { get; set; } =  new();
    public IReadOnlyList<TagInStudySet> TagInStudySets => _tagInStudySets.AsReadOnly();

    public Tag()
    {
        
    }

    public Tag(User user, string name)
    {
        User = user;
        UserId = user.Id;
        Name = name;
        DateCreated = DateTime.UtcNow;
    }
    
    public bool TryAddTrackedEntry(TrackedEntry trackedEntry, LevelStateType? levelStateType = null)
    {
        // check if entryIsTagged already exists with same ent_seq
        var alreadyExists = EntryIsTaggeds.FirstOrDefault(eit => eit.ent_seq == trackedEntry.ent_seq);
        if (alreadyExists != null) return false;
        
        // create EntryIsTagged
        var entryIsTagged = new EntryIsTagged(this, trackedEntry);

        // increment counters
        TotalEntries++;
        switch (trackedEntry.LevelStateType)
        {
            case LevelStateType.New:
                TotalNew++;
                break;
            case LevelStateType.Learning:
                TotalLearning++;
                break;
            case LevelStateType.Reviewing:
                TotalReviewing++;
                break;
            case LevelStateType.Known:
                TotalKnown++;
                break;
            default:
                throw new Exception("Tag.TryAddEntry() Unknown LevelStateType found.");
                break;
        }
        
        // add to tag
        _entryIsTaggeds.Add(entryIsTagged);
        return true;
    }
}