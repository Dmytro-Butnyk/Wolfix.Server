using Seller.Domain.SellerApplicationAggregate.ValueObjects;

namespace Seller.Domain.Projections.SellerApplication;

public sealed record SellerApplicationProjection(Guid Id, string CategoryName,
    string DocumentUrl, SellerProfileData SellerProfileData);