using Shared.Application.Dto;

namespace Support.Application.Dto;

public sealed record SupportForAdminDto(Guid Id, string FirstName, string LastName, string MiddleName) : BaseDto(Id);