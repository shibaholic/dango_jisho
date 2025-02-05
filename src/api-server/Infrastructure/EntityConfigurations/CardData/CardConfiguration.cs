using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities.CardData;
using Domain.Entities.JMDict;

namespace Infrastructure.EntityConfigurations.CardData;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.HasOne(c => c.KanjiElement)
            .WithMany()
            .HasForeignKey(c => c.KanjiId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(c => c.ReadingElement)
            .WithMany()
            .HasForeignKey(c => c.ReadingId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Senses)
            .WithMany()
            .UsingEntity<CardSense>(
                l => l.HasOne<Sense>(cs => cs.Sense).WithMany(s => s.CardSenses).HasForeignKey(cs => cs.SenseId),
                r => r.HasOne<Card>(cs => cs.Card).WithMany(c => c.CardSenses).HasForeignKey(cs => cs.CardId));
    }
}