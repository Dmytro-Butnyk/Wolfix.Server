using Shared.Application.Dto;
using Shared.Domain.ValueObjects;

namespace Seller.Application.Dto.Seller;

public sealed record SellerForAdminDto(Guid Id, string? PhotoUrl, FullName FullName, string PhoneNumber, Address Address,
    DateOnly BirthDate, IReadOnlyCollection<string> Categories) : BaseDto(Id);