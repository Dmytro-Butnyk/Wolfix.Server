using Order.Application.Dto.DeliveryMethod;
using Order.Domain.Projections;

namespace Order.Application.Mapping;

internal static class DeliveryMethodMapper
{
    public static DeliveryMethodDto ToDto(this DeliveryMethodProjection projection)
        => new(projection.Id, projection.Name,
            projection.Cities.Select(city => new CityDto(city.Id, city.Name,
                city.Departments.Select(dp => new DepartmentDto(dp.Id, dp.Number, dp.Street, dp.HouseNumber)),
                city.PostMachines.Select(pm =>
                    new PostMachineDto(pm.Id, pm.Number, pm.Street, pm.HouseNumber, pm.Note)))));
}