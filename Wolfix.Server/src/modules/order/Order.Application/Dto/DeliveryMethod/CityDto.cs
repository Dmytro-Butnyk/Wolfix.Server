namespace Order.Application.Dto.DeliveryMethod;

public sealed record CityDto(Guid Id, string Name, IEnumerable<DepartmentDto> Departments,
    IEnumerable<PostMachineDto> PostMachines);