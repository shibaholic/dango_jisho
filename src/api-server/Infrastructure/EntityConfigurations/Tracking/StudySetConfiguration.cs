using Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Tracking;

public class StudySetConfiguration: IEntityTypeConfiguration<StudySet>
{
    public void Configure(EntityTypeBuilder<StudySet> builder)
    {
        builder.HasKey(ss => ss.Id);
        
        builder.HasOne(ss => ss.User)
            .WithMany(u => u.StudySets)
            .HasForeignKey(ss => ss.UserId)
            .IsRequired();
    }
}

public class TagInStudySetConfiguration : IEntityTypeConfiguration<TagInStudySet>
{
    public void Configure(EntityTypeBuilder<TagInStudySet> builder)
    {
        builder.HasKey(tss => new { tss.TagId, tss.StudySetId });
        
        builder.HasOne(tss => tss.Tag)
            .WithMany(t => t.TagInStudySets)
            .HasForeignKey(tss => tss.TagId)
            .IsRequired();
        
        builder.HasOne(tss => tss.StudySet)
            .WithMany(s => s.TagInStudySets)
            .HasForeignKey(tss => tss.StudySetId)
            .IsRequired();
        
        
    }
}