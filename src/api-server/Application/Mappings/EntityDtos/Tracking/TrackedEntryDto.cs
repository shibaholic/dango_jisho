using System.ComponentModel.DataAnnotations.Schema;
using Application.Mappings.EntityDtos.JMDict;
using Domain.Entities.Tracking;

namespace Application.Mappings.EntityDtos.Tracking;

public class TE_EntryDto
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; }
    public LevelStateType LevelStateType { get; set; } = LevelStateType.New;
    public LevelStateType? OldLevelStateType { get; set; } = null;
    public SpecialCategory? SpecialCategory { get; set; } = null;
    public int Score { get; set; } = 0;
    public DateTime? LastReviewDate { get; set; } = null;
    public TimeSpan? SpacedTime { get; set; } = null;
    public EntryDto? Entry { get; set; }
}

public class TE_Entry_EITDto
{
    public string ent_seq { get; set; }
    public Guid UserId { get; set; }
    public LevelStateType LevelStateType { get; set; } = LevelStateType.New;
    public LevelStateType? OldLevelStateType { get; set; } = null;
    public SpecialCategory? SpecialCategory { get; set; } = null;
    public int Score { get; set; } = 0;
    public DateTime? LastReviewDate { get; set; } = null;
    public TimeSpan? SpacedTime { get; set; } = null;
    public EntryDto? Entry { get; set; }
    public List<EIT_TagDto> EntryIsTaggeds { get; set; } = [];
}

public class TE_EITDto
{
    public LevelStateType LevelStateType { get; set; } = LevelStateType.New;
    public LevelStateType? OldLevelStateType { get; set; } = null;
    public SpecialCategory? SpecialCategory { get; set; } = null;
    public int Score { get; set; } = 0;
    public DateTime? LastReviewDate { get; set; } = null;
    public TimeSpan? SpacedTime { get; set; } = null;
    public List<EIT_TagDto> EntryIsTaggeds { get; set; } = [];
}