using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.ValueGenerators;

namespace Seller.Infrastructure.Configurations;

internal sealed class SellerEntityConfiguration : IEntityTypeConfiguration<Domain.SellerAggregate.Seller>
{
    public void Configure(EntityTypeBuilder<Domain.SellerAggregate.Seller> builder)
    {
        builder.ToTable("Sellers");
        
        ConfigureBasicProperties(builder);
        
        ConfigureSellerCategoriesRelation(builder);
    }

    private void ConfigureBasicProperties(EntityTypeBuilder<Domain.SellerAggregate.Seller> builder)
    {
        const string nullMarker = "_____NULL_____";
        
        builder.Property(s => s.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(s => s.PhotoUrl)
            .IsRequired(false);
        
        builder.Property(s => s.FullName)
            .IsRequired(false)
            .HasConversion<string>(
                fullName => fullName == null ? nullMarker : fullName.ToString(),
                fullNameString => fullNameString == nullMarker ? null : (FullName)fullNameString
            );
        
        builder.Property(s => s.PhoneNumber)
            .IsRequired(false)
            .HasConversion<string>(
                phoneNumber => phoneNumber == null ? nullMarker : phoneNumber.Value,
                phoneNumberString => phoneNumberString == nullMarker ? null : (PhoneNumber)phoneNumberString
            );

        builder.Property(s => s.Address)
            .IsRequired(false)
            .HasConversion<string>(
                address => address == null ? nullMarker : address.ToString(),
                addressString => addressString == nullMarker ? null : (Address)addressString
            );

        builder.Property(s => s.BirthDate)
            .IsRequired(false)
            .HasConversion<string>(
                birthDate => birthDate == null ? nullMarker : birthDate.ToString(),
                birthDateString => birthDateString == nullMarker ? null : (BirthDate)birthDateString
            );
        
        builder.Property(s => s.AccountId)
            .IsRequired();

        builder.Ignore(s => s.SellerCategories);
    }

    private void ConfigureSellerCategoriesRelation(EntityTypeBuilder<Domain.SellerAggregate.Seller> builder)
    {
        builder.HasMany<SellerCategory>("_sellerCategories")
            .WithOne(sc => sc.Seller)
            .HasForeignKey(sc => sc.SellerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        builder.Navigation("_sellerCategories")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}