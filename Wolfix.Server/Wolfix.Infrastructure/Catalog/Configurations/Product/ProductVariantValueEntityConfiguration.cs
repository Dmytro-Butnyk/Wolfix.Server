using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Product;

internal sealed class ProductVariantValueEntityConfiguration : IEntityTypeConfiguration<ProductVariantValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantValue> builder)
    {
        builder.ToTable("ProductVariantValues");
        
        builder.HasOne(pvv => pvv.Product)
            .WithMany("_productVariantValues")
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation("Product")
            .UsePropertyAccessMode(PropertyAccessMode.Property);
        
        builder.Property(pvv => pvv.Key).IsRequired();
        builder.Property(pvv => pvv.Value).IsRequired();
    }
}