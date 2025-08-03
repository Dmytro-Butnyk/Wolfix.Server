using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Category;

internal sealed class ProductAttributeEntityConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");
        
        ConfigureBasicProperties(builder);
        
        ConfigureCategoryRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.Property(pa => pa.Key).IsRequired();
    }

    private void ConfigureCategoryRelation(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasOne(pa => pa.Category)
            .WithMany("_productAttributes")
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(pa => pa.Category)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}