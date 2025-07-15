using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate;
using Wolfix.Domain.Catalog.ProductAggregate;

namespace Wolfix.Infrastructure.TablesConfigurations.CatalogContext.ProductAggregate;

public sealed class ProductConfiguration :  IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.Status)
            .IsRequired();

        // Связь с категорией
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Игнорируем вычисляемое свойство
        builder.Ignore(p => p.FinalPrice);

        // Навигации с приватным полем

        builder.Metadata.FindNavigation(nameof(Product.Resources))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Product.Reviews))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Product.ProductsAttributes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Product.ProductVariants))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}