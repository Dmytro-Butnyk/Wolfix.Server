using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Seller.Domain.SellerAggregate.Entities;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.ValueGenerators;

namespace Seller.Infrastructure.Configurations.Seller;

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
        builder.Property(s => s.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        
        builder.Property(s => s.PhotoUrl)
            .IsRequired(false);
        
        builder.Property(s => s.FullName)
            .IsRequired()
            .HasConversion<string>(
                fullName => fullName.ToString(),
                fullNameString => (FullName)fullNameString
            );
        
        builder.Property(s => s.PhoneNumber)
            .IsRequired()
            .HasConversion<string>(
                phoneNumber => phoneNumber.Value,
                phoneNumberString => (PhoneNumber)phoneNumberString
            );

        builder.Property(s => s.Address)
            .IsRequired()
            .HasConversion<string>(
                address => address.ToString(),
                addressString => (Address)addressString
            );

        builder.Property(s => s.BirthDate)
            .IsRequired()
            .HasConversion<string>(
                birthDate => birthDate.ToString(),
                birthDateString => (BirthDate)birthDateString
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