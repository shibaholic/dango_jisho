using Domain.Entities.JMDict;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.JMDict;

public class SenseConfiguration : IEntityTypeConfiguration<Sense>
{
    public void Configure(EntityTypeBuilder<Sense> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.HasMany(s => s.lsource)
            .WithOne(l => l.Sense)
            .HasForeignKey(l => l.SenseId)
            .IsRequired();
        
        builder.Navigation(s => s.lsource)
            .AutoInclude();
        
        builder.HasIndex(s => s.gloss)
            .HasDatabaseName("IX_Sense_Gloss");
    }
}

public class LSourceConfiguration : IEntityTypeConfiguration<LSource>
{
    public void Configure(EntityTypeBuilder<LSource> builder)
    {
        builder.HasKey(k => k.Id);
    }
}