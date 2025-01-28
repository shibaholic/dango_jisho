using Domain.Entities.JMDict;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.JMDict;

public class ReadingElementConfiguration : IEntityTypeConfiguration<ReadingElement>
{
    public void Configure(EntityTypeBuilder<ReadingElement> builder)
    {
        builder.HasKey(re => re.Id);

        builder.Property(re => re.re_pri)
            .HasConversion(
                re_pri => re_pri.ToString(),
                s => PriorityExtensions.Parse(s)
            );
    }
}