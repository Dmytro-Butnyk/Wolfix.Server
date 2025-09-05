using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Infrastructure.ValueGenerators;

namespace Seller.Infrastructure.Configurations;

internal sealed class SellerCategoryEntityConfiguration : IEntityTypeConfiguration<SellerCategory>
{
    public void Configure(EntityTypeBuilder<SellerCategory> builder)
    {
        builder.ToTable("SellerCategories");
        
        ConfigureBasicProperties(builder);
        
        ConfigureSellerRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<SellerCategory> builder)
    {
        builder.Property(sc => sc.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(sc => sc.CategoryId)
            .IsRequired();

        builder.Property(sc => sc.Name)
            .IsRequired();
    }

    private void ConfigureSellerRelation(EntityTypeBuilder<SellerCategory> builder)
    {
        builder.HasOne<Domain.SellerAggregate.Seller>(sc => sc.Seller)
            .WithMany("_sellerCategories")
            .HasForeignKey(sc => sc.SellerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(sc => sc.Seller)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}