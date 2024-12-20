using Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Tracking;

public class EntryIsTaggedConfiguration : IEntityTypeConfiguration<EntryIsTagged>
{
    public void Configure(EntityTypeBuilder<EntryIsTagged> builder)
    {
        builder.HasKey(eit => new { eit.ent_seq, eit.TagId });
        
        builder.HasOne(eit => eit.Tag)
            .WithMany(t => t.EntryIsTaggeds)
            .HasForeignKey(eit => eit.TagId)
            .IsRequired();

        builder.HasOne(eit => eit.Entry)
            .WithMany(e => e.EntryIsTaggeds)
            .HasForeignKey(eit => eit.ent_seq)
            .IsRequired();

    }
}