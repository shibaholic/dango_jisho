using Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Tracking;

public class ReviewEventConfiguration: IEntityTypeConfiguration<EntryEvent>
{
    public void Configure(EntityTypeBuilder<EntryEvent> builder)
    {
        builder.HasKey(re => new { re.ent_seq, UserId = re.UserId, re.Serial });

        builder.Property(re => re.Serial)
            .ValueGeneratedOnAdd();

        builder.HasOne(re => re.TrackedEntry)
            .WithMany(te => te.EntryEvents)
            .HasForeignKey(re => new { re.ent_seq, TagId = re.UserId })
            .IsRequired();
        
        builder.Property(re => re.Created)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("NOW()");
    }
}