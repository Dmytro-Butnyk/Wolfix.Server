using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.CategoryAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Category;

internal sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<Domain.Catalog.CategoryAggregate.Category>
{
    public void Configure(EntityTypeBuilder<Domain.Catalog.CategoryAggregate.Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey("ParentId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Description).IsRequired(false);
        builder.Property(c => c.ProductsCount).IsRequired();

        //ProductIds
        builder.Ignore(c => c.ProductIds);
        
        //ProductVariants
        builder.Ignore(c => c.ProductVariants);
        
        builder.HasMany<ProductVariant>("_productVariants")
            .WithOne(pv => pv.Category)
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_productVariants")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        //ProductAttributes
        builder.Ignore(c => c.ProductAttributes);
        
        builder.HasMany<ProductAttribute>("_productAttributes")
            .WithOne(pa => pa.Category)
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_productAttributes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}