namespace Shared.Core.Dto;

public sealed record ChangeAddressDto(string City, string Street, uint HouseNumber, uint? ApartmentNumber);