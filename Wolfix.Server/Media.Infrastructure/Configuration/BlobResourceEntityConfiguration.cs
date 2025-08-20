using Media.Domain.BlobAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Media.Infrastructure.Configuration;

public sealed class BlobResourceEntityConfiguration : IEntityTypeConfiguration<BlobResource>
{
    public void Configure(EntityTypeBuilder<BlobResource> builder)
    {
        builder.ToTable("BlobResources");

        ConfigureBasicProperties(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<BlobResource> builder)
    {
        builder.HasKey(br => br.Id);

        builder.Property(br => br.Name)
            .IsRequired();

        builder.Property(br => br.Url)
            .IsRequired();

        builder.Property(br => br.Type)
            .IsRequired()
            .HasConversion<string>();
    }
}