using Catalog.Domain.ProductAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;
using Shared.Domain.Entities;
using Shared.Infrastructure.ValueGenerators;

namespace Catalog.Infrastructure.Configurations.Product;

internal sealed class ProductEntityConfiguration : IEntityTypeConfiguration<Catalog.Domain.ProductAggregate.Product>
{
    public void Configure(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.ToTable("Products");
        
        ConfigureBasicProperties(builder);
        
        ConfigureDiscountRelation(builder);
        
        ConfigureCategoryRelation(builder);
        
        ConfigureReviewsRelation(builder);
        
        ConfigureProductAttributeValuesRelation(builder);
        
        ConfigureProductVariantValuesRelation(builder);

        ConfigureProductMediaRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.Property(p => p.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(p => p.Title).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.FinalPrice).IsRequired();
        builder.Property(p => p.Bonuses).IsRequired();
        builder.Property(p => p.AverageRating).IsRequired(false);
        builder.Property(p => p.Price).IsRequired();
        builder.Property(p => p.Status).IsRequired().HasConversion<string>();
        
        builder.Ignore(r => r.Reviews);
        builder.Ignore(r => r.ProductsAttributeValues);
        builder.Ignore(r => r.ProductVariantValues);
        builder.Ignore(r => r.ProductMedias);
    }
    
    private void ConfigureDiscountRelation(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.HasOne<Discount>(p => p.Discount)
            .WithOne(d => d.Product)
            .HasForeignKey<Discount>(d => d.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation(p => p.Discount)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }

    private void ConfigureCategoryRelation(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.HasOne<Catalog.Domain.CategoryAggregate.Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    private void ConfigureProductMediaRelation(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.HasMany<ProductMedia>("_productMedias")
            .WithOne(pm => pm.Product)
            .HasForeignKey(pm => pm.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation("_productMedias")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureReviewsRelation(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.HasMany<Review>("_reviews")
            .WithOne(r => r.Product)
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_reviews")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureProductAttributeValuesRelation(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.HasMany<ProductAttributeValue>("_productsAttributeValues")
            .WithOne(pav => pav.Product)
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_productsAttributeValues")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureProductVariantValuesRelation(EntityTypeBuilder<Catalog.Domain.ProductAggregate.Product> builder)
    {
        builder.HasMany<ProductVariantValue>("_productVariantValues")
            .WithOne(pvv => pvv.Product)
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_productVariantValues")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}