using Domain.Entities.JMDict;

namespace Domain.Entities.Tracking;

public class TrackedEntry : IBaseEntity
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset? NextReviewDate { get; set; }
    public List<ReviewEvent> ReviewEvents { get; set; } // child nav
    public int Score { get; set; }
    public TrackedStatus Status { get; set; }
    
    // public EntryIsTagged EntryIsTagged { get; set; } // parent nav
    public Entry Entry { get; set; }
    public User User { get; set; }
}

public enum TrackedStatus
{
    New,
    Learning,
    Reviewing,
    Known,
    NeverForget,
    Blacklist,
    FocusReview
}