using Seller.Domain.SellerApplicationAggregate.ValueObjects;

namespace Seller.Application.Dto.SellerApplication;

public sealed record SellerApplicationDto(Guid Id, string CategoryName,
    string DocumentUrl, SellerProfileData SellerProfileData);