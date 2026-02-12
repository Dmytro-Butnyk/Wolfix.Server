using Shared.Application.Dto;

namespace Catalog.Application.Dto.Category.Responses;

public sealed record CategoryFullDto(Guid Id, string Name, string PhotoUrl) : BaseDto(Id);