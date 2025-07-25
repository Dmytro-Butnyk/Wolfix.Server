using Wolfix.Application.Catalog.Dto.Shared;

namespace Wolfix.Application.Catalog.Dto.Category;

public sealed record CategoryShortDto(Guid Id, string Name) : BaseDto(Id);