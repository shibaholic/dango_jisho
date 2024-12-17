using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class KanjiElementConfiguration : IEntityTypeConfiguration<KanjiElement>
{
    public void Configure(EntityTypeBuilder<KanjiElement> builder)
    {
        builder.HasKey(k => k.Id);
    }
}