namespace Domain.Entities.Tracking;

public class StudySet : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime? LastStartDate { get; set; }
    public int NewEntryGoal { get; set; }
    public int NewEntryCount { get; set; }
    // ent_seq of TrackedEntry
    // TODO: validate that they are in fact TrackedEntry, and not just random untracked Entries
    public List<string> NewQueue { get; set; }      = new();
    public List<string> LearningQueue { get; set; } = new();
    public List<string> BaseQueue { get; set; }     = new();
    
    public User User { get; set; }
    public List<TagInStudySet> TagInStudySets { get; set; }
}

public class TagInStudySet : IBaseEntity
{
    public Guid TagId { get; set; }
    public Guid StudySetId { get; set; }
    public int Order { get; set; }
    
    public Tag Tag { get; set; }
    public StudySet StudySet { get; set; }
}