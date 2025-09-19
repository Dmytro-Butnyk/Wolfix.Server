using Customer.Application.Dto.Customer;
using Customer.Domain.Projections;

namespace Customer.Application.Mapping.Customer;

internal static class CustomerMapper
{
    public static CustomerDto ToDto(this CustomerProjection projection)
        => new(projection.Id, projection.PhotoUrl, projection.FullName, projection.PhoneNumber, projection.Address,
            projection.BirthDate, projection.BonusesAmount);
}