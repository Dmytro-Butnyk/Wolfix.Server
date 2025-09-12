namespace Catalog.Application.Dto.Category.Requests;

public sealed record AddChildCategoryDto(string Name, string? Description,
    IReadOnlyCollection<string> AttributeKeys, IReadOnlyCollection<string> VariantKeys);