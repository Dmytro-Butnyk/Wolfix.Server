namespace Catalog.Application.Dto.Product.AdditionDtos;

public sealed record MediaDto(
    string ContentType,
    Stream Filestream);