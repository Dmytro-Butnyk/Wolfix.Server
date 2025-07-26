using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Product;

internal sealed class ProductEntityConfiguration : IEntityTypeConfiguration<Domain.Catalog.ProductAggregate.Product>
{
    public void Configure(EntityTypeBuilder<Domain.Catalog.ProductAggregate.Product> builder)
    {
        builder.ToTable("Products");
        
        builder.Property(p => p.Title).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.Status).IsRequired().HasConversion<string>();
        
        #region Discount
        builder.HasOne<Discount>("Discount")
            .WithOne(d => d.Product)
            .HasForeignKey<Discount>(d => d.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("Discount")
            .UsePropertyAccessMode(PropertyAccessMode.Property);
        #endregion
        
        builder.Ignore(p => p.FinalPrice);
        builder.Ignore(p => p.Bonuses);

        #region Category
        builder.HasOne<Domain.Catalog.CategoryAggregate.Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
        
        //todo: blob resources
        
        #region Reviews
        builder.Ignore(r => r.Reviews);
        
        builder.HasMany<Review>("_reviews")
            .WithOne(r => r.Product)
            .HasForeignKey("ProductId")
            .IsRequired(false);
        builder.Navigation("_reviews")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        #endregion
        
        #region ProductAttributeValues
        builder.Ignore(r => r.ProductsAttributeValues);
        
        builder.HasMany<ProductAttributeValue>("_productsAttributeValues")
            .WithOne(pav => pav.Product)
            .HasForeignKey("ProductId")
            .IsRequired(false);
        builder.Navigation("_productsAttributeValues")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        #endregion
        
        #region ProductVariantValues
        builder.Ignore(r => r.ProductVariantValues);
        
        builder.HasMany<ProductVariantValue>("_productVariantValues")
            .WithOne(pvv => pvv.Product)
            .HasForeignKey("ProductId")
            .IsRequired(false);
        builder.Navigation("_productVariantValues")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        #endregion
    }
}