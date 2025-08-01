using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Category;

internal sealed class ProductVariantEntityConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasOne(pa => pa.Category)
            .WithMany("_productVariants")
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(pa => pa.Category)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
        
        builder.Property(pv => pv.Key).IsRequired();
    }
}