using Catalog.Domain.CategoryAggregate.Entities;
using Catalog.Domain.ProductAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Catalog.Infrastructure.Configurations.Product;

internal sealed class ProductAttributeValueEntityConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");
        
        ConfigureBasicProperties(builder);
        
        ConfigureProductRelation(builder);
        
        ConfigureProductAttributeRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.Property(p => p.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(pav => pav.Key).IsRequired();
        builder.Property(pav => pav.Value).IsRequired(false);
        builder.Property(pav => pav.CategoryAttributeId).IsRequired();
    }
    
    private void ConfigureProductRelation(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.HasOne(pav => pav.Product)
            .WithMany("_productAttributeValues")
            .HasForeignKey(pav => pav.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation("Product")
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }

    private void ConfigureProductAttributeRelation(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.HasOne<ProductAttribute>()
            .WithMany()
            .HasForeignKey(pav => pav.CategoryAttributeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}