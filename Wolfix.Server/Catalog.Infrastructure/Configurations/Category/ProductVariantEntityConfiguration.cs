using Catalog.Domain.CategoryAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Catalog.Infrastructure.Configurations.Category;

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
        builder.Property(p => p.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
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