using Catalog.Domain.ProductAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Configurations.Product;

internal sealed class ProductAttributeValueEntityConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");
        
        ConfigureBasicProperties(builder);
        
        ConfigureProductRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.Property(pav => pav.Key).IsRequired();
        builder.Property(pav => pav.Value).IsRequired();
    }
    
    private void ConfigureProductRelation(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.HasOne(pav => pav.Product)
            .WithMany("_productsAttributeValues")
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation("Product")
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}