using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.TablesConfigurations.CatalogContext.CategoryAggregate;

public sealed class ProductAttributeConfiguration :  IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");

        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Key)
            .IsRequired()
            .HasMaxLength(100); // ограничение длины ключа

        builder.HasOne(pa => pa.Category)
            .WithMany() // если в Category нет навигации к ProductAttributes
            .HasForeignKey("CategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // при удалении категории удалять атрибуты
    }
}