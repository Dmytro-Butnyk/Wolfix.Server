using Seller.Domain.Interfaces;
using Seller.Domain.SellerApplicationAggregate.ValueObjects;
using Shared.Domain.Models;
using SellerEntity = Seller.Domain.SellerAggregate.Seller;

namespace Seller.Domain.Services;

public sealed class SellerDomainService(ISellerRepository sellerRepository)
{
    public async Task<VoidResult> CreateSellerWithFirstCategoryAsync(Guid accountId, SellerProfileData sellerProfileData,
        Guid categoryId, string categoryName, CancellationToken ct)
    {
        Result<SellerEntity> createSellerResult = SellerEntity.Create(accountId, sellerProfileData.FullName.FirstName,
            sellerProfileData.FullName.LastName, sellerProfileData.FullName.MiddleName,
            sellerProfileData.PhoneNumber.Value, sellerProfileData.Address.City, sellerProfileData.Address.Street,
            sellerProfileData.Address.HouseNumber, sellerProfileData.Address.ApartmentNumber,
            sellerProfileData.BirthDate.Value);

        if (createSellerResult.IsFailure)
        {
            return VoidResult.Failure(createSellerResult);
        }

        SellerEntity seller = createSellerResult.Value!;

        VoidResult addSellerCategory = seller.AddSellerCategory(categoryId, categoryName);

        if (addSellerCategory.IsFailure)
        {
            return addSellerCategory;
        }
        
        await sellerRepository.AddAsync(seller, ct);
        await sellerRepository.SaveChangesAsync(ct);
        
        return VoidResult.Success();
    }
}