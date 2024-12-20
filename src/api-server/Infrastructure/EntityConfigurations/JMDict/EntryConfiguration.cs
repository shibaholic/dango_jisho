using Domain.Entities.JMDict;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.JMDict;

public class EntryConfiguration: IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.HasKey(entry => entry.ent_seq);

        builder.HasMany(e => e.KanjiElements)
            .WithOne(k => k.Entry)
            .HasForeignKey(k => k.ent_seq)
            .IsRequired();
        
        builder.HasMany(e => e.ReadingElements)
            .WithOne(k => k.Entry)
            .HasForeignKey(k => k.ent_seq)
            .IsRequired();
        
        builder.HasMany(e => e.Senses)
            .WithOne(k => k.Entry)
            .HasForeignKey(k => k.ent_seq)
            .IsRequired();
    }
}