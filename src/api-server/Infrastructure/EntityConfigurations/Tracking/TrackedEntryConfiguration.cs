using Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Tracking;

public class TrackedEntryConfiguration : IEntityTypeConfiguration<TrackedEntry>
{
    public void Configure(EntityTypeBuilder<TrackedEntry> builder)
    {
        builder.HasKey(te => new { te.ent_seq, te.TagId });
        
        builder.HasOne(te => te.EntryIsTagged)
            .WithOne(eit => eit.TrackedEntry)
            .HasForeignKey<TrackedEntry>(eit => new { eit.ent_seq, eit.TagId })
            .IsRequired();
    }
}