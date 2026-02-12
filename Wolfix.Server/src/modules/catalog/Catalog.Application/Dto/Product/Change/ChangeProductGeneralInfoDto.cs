namespace Catalog.Application.Dto.Product.Change;

public sealed record ChangeProductGeneralInfoDto(string Title, string Description, Guid CategoryId);