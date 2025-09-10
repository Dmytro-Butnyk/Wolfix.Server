using Microsoft.AspNetCore.Http;

namespace Identity.Application.Dto.Requests;

public sealed record RegisterAsSellerDto(
    string Email, string Password, string FirstName, string LastName,
    string MiddleName, string PhoneNumber, string City, string Street, uint HouseNumber,
    uint? ApartmentNumber, DateOnly BirthDate, IFormFile Document);