using Seller.Domain.SellerApplicationAggregate.Enums;
using Seller.Domain.SellerApplicationAggregate.ValueObjects;
using Shared.Domain.Entities;
using Shared.Domain.Models;

namespace Seller.Domain.SellerApplicationAggregate;

public sealed class SellerApplication : BaseEntity
{
    public Guid AccountId { get; private set; }
    
    public string CategoryName { get; private set; }
    
    public Guid BlobResourceId { get; private set; }
    
    public string DocumentUrl { get; private set; }
    
    public SellerProfileData SellerProfileData { get; private set; }

    public SellerApplicationStatus Status { get; private set; } = SellerApplicationStatus.Pending;

    private SellerApplication() { }

    private SellerApplication(Guid accountId, string categoryName, Guid blobResourceId, string documentUrl, SellerProfileData sellerProfileData)
    {
        AccountId = accountId;
        CategoryName = categoryName;
        BlobResourceId = blobResourceId;
        DocumentUrl = documentUrl;
        SellerProfileData = sellerProfileData;
    }

    public static Result<SellerApplication> Create(Guid accountId, string categoryName, Guid blobResourceId, string documentUrl,
        string firstName, string lastName, string middleName, string phoneNumber, string city, string street,
        uint houseNumber, uint? apartmentNumber, DateOnly birthDate)
    {
        if (accountId == Guid.Empty)
        {
            return Result<SellerApplication>.Failure($"{nameof(accountId)} cannot be empty");
        }
        
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return Result<SellerApplication>.Failure($"{nameof(categoryName)} cannot be empty");
        }

        if (blobResourceId == Guid.Empty)
        {
            return Result<SellerApplication>.Failure($"{nameof(blobResourceId)} cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(documentUrl))
        {
            return Result<SellerApplication>.Failure($"{nameof(documentUrl)} cannot be empty");
        }

        Result<SellerProfileData> createSellerProfileDataResult = SellerProfileData.Create(firstName, lastName,
            middleName, phoneNumber, city, street, houseNumber, apartmentNumber, birthDate);

        if (createSellerProfileDataResult.IsFailure)
        {
            return Result<SellerApplication>.Failure(createSellerProfileDataResult);
        }

        SellerProfileData sellerProfileData = createSellerProfileDataResult.Value!;

        return Result<SellerApplication>.Success(new(accountId, categoryName,
            blobResourceId,documentUrl, sellerProfileData));
    }

    public VoidResult Approve()
    {
        if (Status != SellerApplicationStatus.Pending)
        {
            return VoidResult.Failure("Application is already approved or rejected");
        }
        
        Status = SellerApplicationStatus.Approved;
        
        return VoidResult.Success();
    }
    
    public VoidResult Reject()
    {
        if (Status != SellerApplicationStatus.Pending)
        {
            return VoidResult.Failure("Application is already approved or rejected");
        }
        
        Status = SellerApplicationStatus.Rejected;
        
        return VoidResult.Success();
    }
}