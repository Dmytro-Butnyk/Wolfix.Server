using Wolfix.Application.Shared.Dto;

namespace Wolfix.Application.Catalog.Dto.Category;

public sealed record CategoryShortDto(Guid Id, string Name) : BaseDto(Id);