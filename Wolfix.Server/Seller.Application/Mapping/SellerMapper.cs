using Seller.Application.Dto;
using Seller.Domain.Projections;

namespace Seller.Application.Mapping;

internal static class SellerMapper
{
    public static SellerDto ToDto(this SellerProjection projection)
        => new(projection.Id, projection.PhotoUrl, projection.FullName, projection.PhoneNumber,
            projection.Address, projection.BirthDate);
}