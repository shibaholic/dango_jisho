using Domain.Entities.Tracking;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Tracking;

public class TrackedEntryConfiguration : IEntityTypeConfiguration<TrackedEntry>
{
    public void Configure(EntityTypeBuilder<TrackedEntry> builder)
    {
        builder.HasKey(te => new { te.ent_seq, te.UserId });
        
        builder.HasOne(te => te.Entry)
            .WithMany(e => e.TrackedEntries)
            .HasForeignKey(te => te.ent_seq)
            .IsRequired();
        
        builder.HasOne(te => te.User)
            .WithMany(u => u.TrackedEntries)
            .HasForeignKey(te => te.UserId)
            .IsRequired();

        builder.Property(te => te.Score)
            .HasDefaultValue(0);

        // builder.Property(te => te.LastReviewDate)
        //     .HasColumnType("timestamptz")
        //     .HasDefaultValueSql("NOW()");

        builder.Property(te => te.LevelStateType)
            .HasConversion(
                v => v.ToString(),
                s => EnumExtension.Parse<LevelStateType>(s)
            );
        
        builder.Property(te => te.OldLevelStateType)
            .HasConversion(
                v => v.ToString(),
                s => EnumExtension.Parse<LevelStateType>(s)
            );
        
        builder.Property(te => te.SpecialCategory)
            .HasConversion(
                v => v.ToString(),
                s => EnumExtension.Parse<SpecialCategory>(s)
            );
    }
}