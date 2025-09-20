using Seller.Application.Dto.SellerApplication;
using Seller.Domain.Projections.SellerApplication;

namespace Seller.Application.Mapping.SellerApplication;

internal static class SellerApplicationMapper
{
    public static SellerApplicationDto ToDto(this SellerApplicationProjection projection)
        => new(projection.Id, projection.CategoryName, projection.DocumentUrl, projection.SellerProfileData);
}