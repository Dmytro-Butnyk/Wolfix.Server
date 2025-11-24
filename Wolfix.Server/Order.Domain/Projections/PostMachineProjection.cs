namespace Order.Domain.Projections;

public sealed record PostMachineProjection(Guid Id, uint Number, string Street, uint HouseNumber, string? Note);