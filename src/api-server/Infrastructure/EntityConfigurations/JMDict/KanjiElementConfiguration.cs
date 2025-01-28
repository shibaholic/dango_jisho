using Domain.Entities.JMDict;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.JMDict;

public class KanjiElementConfiguration : IEntityTypeConfiguration<KanjiElement>
{
    public void Configure(EntityTypeBuilder<KanjiElement> builder)
    {
        builder.HasKey(k => k.Id);
        
        builder.Property(ke => ke.ke_pri)
            .HasConversion(
                ke_pri => ke_pri.ToString(),
                s => EnumExtension.Parse<Priority>(s)
            );
    }
}