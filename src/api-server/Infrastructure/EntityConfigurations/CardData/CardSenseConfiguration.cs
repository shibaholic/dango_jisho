using Domain.Entities.CardData;
using Domain.Entities.JMDict;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.CardData;

public class CardSenseConfiguration : IEntityTypeConfiguration<CardSense>
{
    public void Configure(EntityTypeBuilder<CardSense> builder)
    {
        builder.HasKey(cs => new {cs.CardId, cs.SenseId});

        builder.HasOne(cs => cs.Card)
            .WithMany(c => c.CardSenses)
            .HasForeignKey(cs => cs.CardId)
            .IsRequired();
        
        builder.HasOne(cs => cs.Sense)
            .WithMany(s => s.CardSenses)
            .HasForeignKey(cs => cs.SenseId)
            .IsRequired();
    }
}