using Domain.Entities.Tracking;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Tracking;

public class EntryEventConfiguration: IEntityTypeConfiguration<EntryEvent>
{
    public void Configure(EntityTypeBuilder<EntryEvent> builder)
    {
        builder.HasKey(ee => new { ee.ent_seq, UserId = ee.UserId, ee.Serial });

        builder.Property(ee => ee.Serial)
            .ValueGeneratedOnAdd();

        builder.HasOne(ee => ee.TrackedEntry)
            .WithMany(te => te.EntryEvents)
            .HasForeignKey(re => new { re.ent_seq, TagId = re.UserId })
            .IsRequired();
        
        builder.Property(ee => ee.Created)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("NOW()");
        
        builder.Property(ee => ee.ReviewValue)
            .HasConversion(
                v => v.ToString(),
                s => EnumExtension.Parse<ReviewValue>(s)
            );
        
        builder.Property(ee => ee.ChangeValue)
            .HasConversion(
                v => v.ToString(),
                s => EnumExtension.Parse<ChangeValue>(s)
            );
    }
}