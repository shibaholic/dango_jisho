using Domain.Entities.JMDict;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.JMDict;

public class ReadingElementConfiguration : IEntityTypeConfiguration<ReadingElement>
{
    public void Configure(EntityTypeBuilder<ReadingElement> builder)
    {
        builder.HasKey(s => s.Id);
    }
}