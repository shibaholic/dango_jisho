using Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Tracking;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tags)
            .HasForeignKey(t => t.UserId)
            .IsRequired(false);

        builder.Property(t => t.Name)
            .IsRequired();
        
        // builder.Property(t => t.DateCreated)
        //     .HasColumnType("timestamptz")
        //     .HasDefaultValueSql("NOW()");
        
        builder.Property(t => t.TotalEntries)
            .HasDefaultValue(0);
    }
}