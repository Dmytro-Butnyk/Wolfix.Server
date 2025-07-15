using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate;

namespace Wolfix.Infrastructure.TablesConfigurations.CatalogContext.CategoryAggregate;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    //todo: add table validation
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(500); // Опционально, если нужна ограниченная длина

        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey("ParentId")
            .OnDelete(DeleteBehavior.Restrict);

        // Игнорируем коллекцию _productIds, так как это вспомогательная проекция
        builder.Ignore(c => c.ProductIds);

        // ProductVariants маппится к полю напрямую (приватный список)
        builder.Metadata
            .FindNavigation(nameof(Category.ProductVariants))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}