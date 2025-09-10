namespace Customer.Application.Dto.Customer;

public sealed record AddressDto(string City, string Street, uint HouseNumber, uint? ApartmentNumber);