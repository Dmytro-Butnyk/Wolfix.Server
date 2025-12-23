using Shared.Application.Dto;

namespace Admin.Application.Dto.Responses;

public sealed record BasicAdminDto(Guid Id, string FirstName, string LastName, string MiddleName, string PhoneNumber) : BaseDto(Id);