using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Category;

internal sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<Domain.Catalog.CategoryAggregate.Category>
{
    public void Configure(EntityTypeBuilder<Domain.Catalog.CategoryAggregate.Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey("ParentId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Description).IsRequired(false);
        
        builder.HasMany<Domain.Catalog.ProductAggregate.Product>("_productIds")
            .WithOne()
            .HasForeignKey("CategoryId")
            .IsRequired(false);
        builder.Navigation("_productIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany<ProductVariant>("_productVariants")
            .WithOne(pv => pv.Category)
            .HasForeignKey("CategoryId")
            .IsRequired(false);
        builder.Navigation("_productVariants")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany<ProductAttribute>("_productAttributes")
            .WithOne(pa => pa.Category)
            .HasForeignKey("CategoryId")
            .IsRequired(false);
        builder.Navigation("_productAttributes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(c => c.ProductsCount);
    }
}