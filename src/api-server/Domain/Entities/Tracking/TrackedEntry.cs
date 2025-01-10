using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.JMDict;

namespace Domain.Entities.Tracking;

public class TrackedEntry : IBaseEntity
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; }
    public bool Ready { get; set; } = false; // what was the purpose of this field again?
    public LevelStateType LevelStateType { get; set; } = LevelStateType.New;
    [NotMapped] public ILevelState? LevelState { get; private set; } = null; // not stored in DB

    public SpecialCategory? SpecialCategory { get; set; } = null;
    public int Score { get; set; } = 0;
    public DateTime? LastReviewDate { get; set; } = null;
    public int? NextReviewDays { get; set; } = null;
    public int? NextReviewMinutes { get; set; } = null;
    public List<EntryEvent> EntryEvents { get; set; } = new List<EntryEvent>(); // child nav
    
    public Entry Entry { get; set; } // parent
    public User User { get; set; } // parent
    public List<EntryIsTagged> EntryIsTaggeds { get; set; } = new List<EntryIsTagged>();

    public void SetLevelState(ILevelState levelState)
    {
        this.LevelState = levelState;
        this.LevelState.SetContext(this);
    }

    public void UpdateBasedOnEntryEvent(EntryEvent entryEvent, DateTime now)
    {
        Console.WriteLine("[EntryEvent] UpdateBasedOnEntryEvent");
        if(LevelState == null) throw new NullReferenceException("Use SetLevelState to set LevelState before calling.");
        LevelState.UpdateBasedOnEntryEvent(entryEvent, now);
        
        EntryEvents.Add(entryEvent);
    }
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

public interface ILevelState
{
    public TrackedEntry TrackedEntry { get; set; }
    public void UpdateBasedOnEntryEvent(EntryEvent entryEvent, DateTime now);
    public void CheckReviewDate();

    public void SetContext(TrackedEntry trackedEntry)
    {
        this.TrackedEntry = trackedEntry;
    }
}

public class LevelStateNew : ILevelState
{
    public TrackedEntry TrackedEntry { get; set; }
    public void UpdateBasedOnEntryEvent(EntryEvent entryEvent, DateTime now)
    {
        Console.WriteLine("[LevelStateNew] UpdateBasedOnEntryEvent");
        
        // This is the initial entry event when first seeing this card.
        // Right now, I will only implement it going straight to Learning score 0. Which is EntryEvent.
        // TODO: implement 1 day Review, 5 day Know, 10 day Know.
        
        // no matter what Value, make state = Learning and score = 0;
        TrackedEntry.LevelStateType = LevelStateType.Learning;
        TrackedEntry.Score = 0;
        // don't forget to set NextReviewMinutes and LastReview
        // TODO: implement user defined Minutes and Score curves.
        TrackedEntry.NextReviewMinutes = 5;
        TrackedEntry.LastReviewDate = now;
    }

    public void CheckReviewDate() // what was this for again?
    {
        
    }
}

public class LevelStateLearning : ILevelState
{
    public TrackedEntry TrackedEntry { get; set; }
    public void UpdateBasedOnEntryEvent(EntryEvent entryEvent, DateTime now)
    {
        Console.WriteLine("[LevelStateLearning] UpdateBasedOnEntryEvent");
        if (entryEvent.EventType == EventType.Change)
        {
            // read the ChangeValue and change depending on that (this is same to all ILevelStates !!!!!!!
            
        } else if (entryEvent.EventType == EventType.Review)
        {
            // read the ReviewValue
            if (entryEvent.ReviewValue == ReviewValue.Okay)
            {
                // increment Score by 1
                TrackedEntry.Score += 1;
                // change nextReviewMinutes
                TrackedEntry.NextReviewMinutes = 10;
                TrackedEntry.LastReviewDate = now;
            }
        }
        else
        {
            throw new ArgumentException("Unknown EntryEvent.EventType");
        }
    }

    public void CheckReviewDate()
    {
        
    }
}

public class LevelStateReview : ILevelState
{
    public TrackedEntry TrackedEntry { get; set; }
    public void UpdateBasedOnEntryEvent(EntryEvent entryEvent, DateTime now)
    {
        Console.WriteLine("[LevelStateReview] UpdateBasedOnEntryEvent");
    }

    public void CheckReviewDate()
    {
        
    }
}

public class LevelStateKnown : ILevelState
{
    public TrackedEntry TrackedEntry { get; set; }
    public void UpdateBasedOnEntryEvent(EntryEvent entryEvent, DateTime now)
    {
        Console.WriteLine("[LevelStateKnown] UpdateBasedOnEntryEvent");
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
            LevelStateType.Learning => new LevelStateLearning(),
            LevelStateType.Reviewing => new LevelStateReview(),
            LevelStateType.Known => new LevelStateKnown(),
            _ => throw new ArgumentException("Invalid level state type")
        };
    }
}