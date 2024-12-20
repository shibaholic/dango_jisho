using Domain.Entities.JMDict;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.JMDict;

public class KanjiElementConfiguration : IEntityTypeConfiguration<KanjiElement>
{
    public void Configure(EntityTypeBuilder<KanjiElement> builder)
    {
        builder.HasKey(k => k.Id);
    }
}