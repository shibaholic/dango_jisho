namespace Application.Mappings.EntityDtos.Tracking;

public class StudySetDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime? LastStartDate { get; set; }
    public int NewEntryGoal { get; set; }
    public int NewEntryCount { get; set; }
    public List<string> NewQueue { get; set; } = new List<string>();      // ent_seq of TrackedEntry
    public List<string> LearningQueue { get; set; } = new List<string>(); // TODO: validate that they are in fact TrackedEntry, and not just random untracked Entries
    public List<string> BaseQueue { get; set; } = new List<string>();
}