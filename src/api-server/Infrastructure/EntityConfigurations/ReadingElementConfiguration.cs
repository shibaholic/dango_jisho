using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations;

public class ReadingElementConfiguration : IEntityTypeConfiguration<ReadingElement>
{
    public void Configure(EntityTypeBuilder<ReadingElement> builder)
    {
        builder.HasKey(k => k.Id);
    }
}