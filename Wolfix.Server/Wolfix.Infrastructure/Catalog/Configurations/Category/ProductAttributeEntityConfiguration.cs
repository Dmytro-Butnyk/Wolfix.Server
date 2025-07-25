using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Category;

internal sealed class ProductAttributeEntityConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");
        
        builder.HasOne(pa => pa.Category)
            .WithMany("_productAttributes")
            .HasForeignKey("CategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(pa => pa.Category)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(pa => pa.Key).IsRequired();
    }
}