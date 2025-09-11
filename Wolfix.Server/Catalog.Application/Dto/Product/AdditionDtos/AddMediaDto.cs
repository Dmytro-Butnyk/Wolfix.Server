using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Dto.Product.AdditionDtos;

public sealed record AddMediaDto(
    string ContentType,
    IFormFile Filestream);