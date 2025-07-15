using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.TablesConfigurations.CatalogContext.ProductAggregate;

public sealed class ProductAttributeValueConfiguration :  IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");

        builder.HasKey(pav => pav.Id);

        builder.Property(pav => pav.Key)
            .IsRequired()
            .HasMaxLength(100); // например, ограничение длины ключа

        builder.Property(pav => pav.Value)
            .IsRequired()
            .HasMaxLength(500); // ограничение длины значения

        builder.HasOne(pav => pav.Product)
            .WithMany(p => p.ProductsAttributes)  // Навигационное свойство в Product
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // При удалении продукта - удаляются атрибуты
    }
}