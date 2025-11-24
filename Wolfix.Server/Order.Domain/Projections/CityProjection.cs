namespace Order.Domain.Projections;

public sealed record CityProjection(Guid Id, string Name, IEnumerable<DepartmentProjection> Departments,
    IEnumerable<PostMachineProjection> PostMachines);