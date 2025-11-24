namespace Order.Application.Dto.DeliveryMethod;

public sealed record PostMachineDto(Guid Id, uint Number, string Street, uint HouseNumber, string? Note);