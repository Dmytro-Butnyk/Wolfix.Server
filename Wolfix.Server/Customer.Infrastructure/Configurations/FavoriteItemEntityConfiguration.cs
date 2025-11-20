using Customer.Domain.CustomerAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.ValueGenerators;

namespace Customer.Infrastructure.Configurations;

internal sealed class FavoriteItemEntityConfiguration : IEntityTypeConfiguration<FavoriteItem>
{
    public void Configure(EntityTypeBuilder<FavoriteItem> builder)
    {
        builder.ToTable("FavoriteItems");
        
        ConfigureBasicProperties(builder);
        
        ConfigureCustomerRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<FavoriteItem> builder)
    {
        builder.Property(p => p.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(fi => fi.PhotoUrl)
            .IsRequired();
        
        builder.Property(fi => fi.Title)
            .IsRequired();

        builder.Property(fi => fi.AverageRating)
            .IsRequired(false);

        builder.Property(fi => fi.Price)
            .IsRequired();

        builder.Property(fi => fi.FinalPrice)
            .IsRequired(false);

        builder.Property(fi => fi.Bonuses)
            .IsRequired();
    }

    private void ConfigureCustomerRelation(EntityTypeBuilder<FavoriteItem> builder)
    {
        builder.HasOne<Customer.Domain.CustomerAggregate.Customer>(fi => fi.Customer)
            .WithMany("_favoriteItems")
            .HasForeignKey(fi => fi.CustomerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.Navigation(fi => fi.Customer)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}