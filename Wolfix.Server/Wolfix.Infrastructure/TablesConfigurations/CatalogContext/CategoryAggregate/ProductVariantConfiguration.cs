using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.TablesConfigurations.CatalogContext.CategoryAggregate;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Key)
            .IsRequired()
            .HasMaxLength(50);  // Можно скорректировать длину

        builder.HasOne(pv => pv.Category)
            .WithMany(c => c.ProductVariants)
            .HasForeignKey("CategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);  // Если удалили Category — удалятся все ProductVariants
    }
}