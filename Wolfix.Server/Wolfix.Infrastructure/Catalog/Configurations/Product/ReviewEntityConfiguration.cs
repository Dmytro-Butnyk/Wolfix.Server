using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Product;

internal sealed class ReviewEntityConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        
        ConfigureBasicProperties(builder);

        ConfigureProductRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<Review> builder)
    {
        builder.Property(r => r.Title).IsRequired();
        builder.Property(r => r.Text).IsRequired();
        builder.Property(r => r.Rating).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
    }

    private void ConfigureProductRelation(EntityTypeBuilder<Review> builder)
    {
        builder.HasOne(r => r.Product)
            .WithMany("_reviews")
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation("Product")
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}