using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Product;

internal sealed class ProductEntityConfiguration : IEntityTypeConfiguration<Domain.Catalog.ProductAggregate.Product>
{
    public void Configure(EntityTypeBuilder<Domain.Catalog.ProductAggregate.Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Title).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.Status).IsRequired().HasConversion<string>();
        
        //Discount
        builder.HasOne(typeof(Discount), "Discount")
            .WithOne()
            .HasForeignKey(typeof(Discount), "ProductId")
            .IsRequired(false);
        builder.Navigation("Discount")
            .UsePropertyAccessMode(PropertyAccessMode.Property);
        
        builder.Ignore(p => p.FinalPrice);
        builder.Ignore(p => p.Bonuses);

        //Category
        builder.HasOne<Domain.Catalog.CategoryAggregate.Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        //todo: blob resources
        
        //Reviews
        builder.HasMany<Review>("_reviews")
            .WithOne(r => r.Product)
            .HasForeignKey("ProductId")
            .IsRequired(false);
        builder.Navigation("_reviews")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        //ProductAttributeValues
        builder.HasMany<ProductAttributeValue>("_productsAttributeValues")
            .WithOne(pav => pav.Product)
            .HasForeignKey("ProductId")
            .IsRequired(false);
        builder.Navigation("_productsAttributeValues")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        //ProductVariantValues
        builder.HasMany<ProductVariantValue>("_productVariantValues")
            .WithOne(pvv => pvv.Product)
            .HasForeignKey("ProductId")
            .IsRequired(false);
        builder.Navigation("_productVariantValues")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}