using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Category;

internal sealed class ProductVariantEntityConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        
        ConfigureBasicProperties(builder);

        ConfigureCategoryRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.Property(pv => pv.Key).IsRequired();
    }
    
    private void ConfigureCategoryRelation(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasOne(pa => pa.Category)
            .WithMany("_productVariants")
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(pa => pa.Category)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}