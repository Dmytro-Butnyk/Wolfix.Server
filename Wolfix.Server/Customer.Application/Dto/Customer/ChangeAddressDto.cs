namespace Customer.Application.Dto.Customer;

public sealed record ChangeAddressDto(string City, string Street, uint HouseNumber, uint? ApartmentNumber);