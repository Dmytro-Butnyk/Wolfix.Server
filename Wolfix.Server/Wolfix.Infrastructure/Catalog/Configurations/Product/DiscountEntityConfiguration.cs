using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wolfix.Domain.Catalog.ProductAggregate.Entities;

namespace Wolfix.Infrastructure.Catalog.Configurations.Product;

internal sealed class DiscountEntityConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("Discounts");
        
        builder.Property(d => d.Percent).IsRequired();
        builder.Property(d => d.ExpirationDateTime).IsRequired();
        builder.Property(d => d.Status).IsRequired().HasConversion<string>();
        
        #region Product
        builder.HasOne<Domain.Catalog.ProductAggregate.Product>(d => d.Product)
            .WithOne("Discount")
            .HasForeignKey<Discount>(d => d.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation(d => d.Product)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
        #endregion
    }
}