using Shared.Application.Dto;

namespace Catalog.Application.Dto.Category;

public sealed record CategoryShortDto(Guid Id, string Name) : BaseDto(Id);