using Seller.Application.Dto.Seller;
using Seller.Domain.Projections.Seller;

namespace Seller.Application.Mapping.Seller;

internal static class SellerMapper
{
    public static SellerDto ToDto(this SellerProjection projection)
        => new(projection.Id, projection.PhotoUrl, projection.FullName, projection.PhoneNumber,
            projection.Address, projection.BirthDate);
}