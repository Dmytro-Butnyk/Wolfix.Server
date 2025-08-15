using Catalog.Domain.ProductAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Configurations.Product;

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
            .HasForeignKey(r => r.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(r => r.Product)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}