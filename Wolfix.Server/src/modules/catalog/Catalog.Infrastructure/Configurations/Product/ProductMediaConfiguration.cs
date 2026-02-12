using Catalog.Domain.ProductAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Enums;

namespace Catalog.Infrastructure.Configurations.Product;

internal sealed class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.ToTable("ProductMedias");
        
        ConfigureBasicProperties(builder);

        ConfigureProductRelation(builder);
    }
    
    private void ConfigureBasicProperties(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.Property(pm => pm.MediaId).IsRequired();
        builder.Property(pm => pm.MediaType)
            .HasConversion<string>(mediaType => mediaType.ToString(),
                mediaTypeString => Enum.Parse<BlobResourceType>(mediaTypeString))
            .IsRequired();
        builder.Property(r => r.MediaUrl).IsRequired();
    }
    
    private void ConfigureProductRelation(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.HasOne(r => r.Product)
            .WithMany("_productMedias")
            .HasForeignKey(r => r.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.Product)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}