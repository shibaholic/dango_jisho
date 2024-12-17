using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class EntryConfiguration: IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.HasKey(entry => entry.ent_seq);

        builder.HasMany(e => e.KanjiElementNavs)
            .WithOne(k => k.Entry)
            .HasForeignKey(k => k.ent_seq)
            .IsRequired();
        
        builder.HasMany(e => e.ReadingElementNavs)
            .WithOne(k => k.Entry)
            .HasForeignKey(k => k.ent_seq)
            .IsRequired();
        
        builder.HasMany(e => e.SenseNavs)
            .WithOne(k => k.Entry)
            .HasForeignKey(k => k.ent_seq)
            .IsRequired();
    }
}