using Catalog.Domain.CategoryAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Catalog.Infrastructure.Configurations.Category;

internal sealed class CategoryEntityConfiguration : IEntityTypeConfiguration<Catalog.Domain.CategoryAggregate.Category>
{
    public void Configure(EntityTypeBuilder<Catalog.Domain.CategoryAggregate.Category> builder)
    {
        builder.ToTable("Categories");
        
        ConfigureBasicProperties(builder);
        
        ConfigureParentRelation(builder);
        ConfigureProductVariantsRelation(builder);
        ConfigureProductAttributesRelation(builder);
    }

    private void ConfigureParentRelation(EntityTypeBuilder<Catalog.Domain.CategoryAggregate.Category> builder)
    {
        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey("ParentId")
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(c => c.Parent)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<Catalog.Domain.CategoryAggregate.Category> builder)
    {
        builder.Property(p => p.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();

        builder.Property(c => c.BlobResourceId).IsRequired();
        builder.Property(c => c.PhotoUrl).IsRequired();
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Description).IsRequired(false);
        builder.Property(c => c.ProductsCount).IsRequired();

        builder.Ignore(c => c.IsParent);
        builder.Ignore(c => c.IsChild);
        builder.Ignore(c => c.ProductIds);
        builder.Ignore(c => c.ProductVariants);
        builder.Ignore(c => c.ProductAttributes);
    }

    private void ConfigureProductVariantsRelation(EntityTypeBuilder<Catalog.Domain.CategoryAggregate.Category> builder)
    {
        builder.HasMany<ProductVariant>("_productVariants")
            .WithOne(pv => pv.Category)
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_productVariants")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureProductAttributesRelation(EntityTypeBuilder<Catalog.Domain.CategoryAggregate.Category> builder)
    {
        builder.HasMany<ProductAttribute>("_productAttributes")
            .WithOne(pa => pa.Category)
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_productAttributes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}