using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.JMDict;

namespace Domain.Entities.Tracking;

public class TrackedEntry : IBaseEntity
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; }
    public bool Ready { get; set; } = false; // what was the purpose of this field again?
    public LevelStateType LevelStateType { get; set; } = LevelStateType.New;
    [NotMapped]
    public ILevelState LevelState { get; private set; } // not stored in DB

    public SpecialCategory? SpecialCategory { get; set; } = null;
    public int Score { get; set; }
    public DateTime? LastReviewDate { get; set; } = null;
    public int? NextReviewDays { get; set; } = null;
    public int? NextReviewMinutes { get; set; } = null;
    public List<ReviewEvent> ReviewEvents { get; set; } // child nav
    
    public Entry Entry { get; set; } // parent
    public User User { get; set; } // parent
    public List<EntryIsTagged> EntryIsTaggeds { get; set; } = new List<EntryIsTagged>();
}

public interface ILevelState
{
    public void AddReviewEvent(ReviewEvent reviewEvent);
    public void CheckReviewDate();
}

public enum LevelStateType
{
    New,
    Learning,
    Reviewing,
    Known
}

public enum SpecialCategory
{
    NeverForget,
    Blacklist,
    Cram
}

public class LevelStateNew : ILevelState
{
    public void AddReviewEvent(ReviewEvent reviewEvent)
    {
        
    }

    public void CheckReviewDate()
    {
        
    }
}

public static class LevelStateFactory
{
    public static ILevelState Create(LevelStateType levelStateType)
    {
        return levelStateType switch
        {
            LevelStateType.New => new LevelStateNew(),
            _ => throw new ArgumentException("Invalid level state type")
        };
    }
}