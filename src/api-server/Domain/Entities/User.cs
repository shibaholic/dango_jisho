using Domain.Entities.Tracking;

namespace Domain.Entities;

public class User : IBaseEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public Guid? RefreshToken { get; set; }
    public bool IsAdmin { get; set; } = false;
    
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public List<TrackedEntry> TrackedEntries { get; set; } = new List<TrackedEntry>();
    public List<StudySet> StudySets { get; set; } = new List<StudySet>();
    
    public void GenerateRefreshToken()
    {
        RefreshToken = Guid.NewGuid();
    }
}