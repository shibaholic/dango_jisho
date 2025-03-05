using System.ComponentModel.DataAnnotations.Schema;
using Application.Mappings.EntityDtos.JMDict;
using Domain.Entities.Tracking;

namespace Application.Mappings.EntityDtos.Tracking;

public class TrackedEntryDto
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; }
    public LevelStateType LevelStateType { get; set; } = LevelStateType.New;
    public LevelStateType? OldLevelStateType { get; set; } = null;
    public SpecialCategory? SpecialCategory { get; set; } = null;
    public int Score { get; set; } = 0;
    public DateTime? LastReviewDate { get; set; } = null;
    public int? NextReviewDays { get; set; } = null;
    public int? NextReviewMinutes { get; set; } = null;
    public EntryDto? Entry { get; set; }
}