using Microsoft.AspNetCore.Http;

namespace Seller.Application.Dto.SellerApplication;

public sealed record CreateSellerApplicationDto(string FirstName, string LastName, string MiddleName, string PhoneNumber, string City,
    string Street, uint HouseNumber, uint? ApartmentNumber, DateOnly BirthDate, Guid CategoryId, string CategoryName, IFormFile Document);