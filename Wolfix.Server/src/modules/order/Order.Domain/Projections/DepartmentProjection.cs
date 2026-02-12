namespace Order.Domain.Projections;

public sealed record DepartmentProjection(Guid Id, uint Number, string Street, uint HouseNumber);