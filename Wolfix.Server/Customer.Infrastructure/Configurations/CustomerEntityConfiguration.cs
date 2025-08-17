using Customer.Domain.CustomerAggregate.Entities;
using Customer.Domain.CustomerAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Configurations;

internal sealed class CustomerEntityConfiguration : IEntityTypeConfiguration<Domain.CustomerAggregate.Customer>
{
    public void Configure(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        builder.ToTable("Customers");
        
        ConfigureBasicProperties(builder);
        
        ConfigureFavoriteItemsRelation(builder);
        
        ConfigureCartItemsRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        builder.Property(c => c.FullName)
            .IsRequired()
            .HasConversion<string>(
                fullName => fullName.ToString(),
                fullNameString => (FullName)fullNameString
            );
        
        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasConversion<string>(
                phoneNumber => phoneNumber.Value,
                phoneNumberString => (PhoneNumber)phoneNumberString
            );

        builder.Property(c => c.Address)
            .IsRequired()
            .HasConversion<string>(
                address => address.ToString(),
                addressString => (Address)addressString
            );

        builder.Property(c => c.BirthDate)
            .IsRequired()
            .HasConversion<string>(
                birthDate => birthDate.ToString(),
                birthDateString => (BirthDate)birthDateString
            );

        builder.Property(c => c.BonusesAmount)
            .IsRequired();

        builder.Property(c => c.AccountId)
            .IsRequired();

        builder.Ignore(c => c.FavoriteItems);

        builder.Ignore(c => c.CartItems);
    }
    
    private void ConfigureFavoriteItemsRelation(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        builder.HasMany<FavoriteItem>("_favoriteItems")
            .WithOne(fi => fi.Customer)
            .HasForeignKey(fi => fi.CustomerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_favoriteItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureCartItemsRelation(EntityTypeBuilder<Domain.CustomerAggregate.Customer> builder)
    {
        builder.HasMany<CartItem>("_cartItems")
            .WithOne(ci => ci.Customer)
            .HasForeignKey(ci => ci.CustomerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_cartItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}