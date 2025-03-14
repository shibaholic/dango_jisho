namespace Domain.Entities.Tracking;

public class EntryEvent : IBaseEntity
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; }
    public int Serial { get; set; }
    public DateTime Created { get; set; }
    public EventType EventType { get; set; }
    public ReviewValue? ReviewValue { get; set; }
    public ChangeValue? ChangeValue { get; set; }
    
    public TrackedEntry TrackedEntry { get; set; } // parent nav
}

public enum EventType
{
    Review,
    Change
}

public enum ReviewValue
{
    New,
    Again,
    Soon,
    Okay,
    Easy
}

public enum ChangeValue
{
    New,
    Learning,
    Reviewing,
    Known
}