namespace Shared.Core.Dto;

public sealed record AddressDto(string City, string Street, uint HouseNumber, uint? ApartmentNumber);