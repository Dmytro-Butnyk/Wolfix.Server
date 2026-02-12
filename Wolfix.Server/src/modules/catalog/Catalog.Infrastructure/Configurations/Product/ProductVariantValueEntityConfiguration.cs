using Catalog.Domain.CategoryAggregate.Entities;
using Catalog.Domain.ProductAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Catalog.Infrastructure.Configurations.Product;

internal sealed class ProductVariantValueEntityConfiguration : IEntityTypeConfiguration<ProductVariantValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantValue> builder)
    {
        builder.ToTable("ProductVariantValues");
        
        ConfigureBasicProperties(builder);
        
        ConfigureProductRelation(builder);
        
        ConfigureProductVariantRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<ProductVariantValue> builder)
    {
        builder.Property(p => p.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(pvv => pvv.Key).IsRequired();
        builder.Property(pvv => pvv.Value).IsRequired(false);
    }

    private void ConfigureProductRelation(EntityTypeBuilder<ProductVariantValue> builder)
    {
        builder.HasOne(pvv => pvv.Product)
            .WithMany("_productVariantValues")
            .HasForeignKey(pvv => pvv.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(pvv => pvv.Product)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }

    private void ConfigureProductVariantRelation(EntityTypeBuilder<ProductVariantValue> builder)
    {
        builder.HasOne<ProductVariant>()
            .WithMany()
            .HasForeignKey(pvv => pvv.CategoryVariantId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}