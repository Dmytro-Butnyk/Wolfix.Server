using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Dto.Category.Requests;

public sealed record AddParentCategoryDto(IFormFile Photo, string Name, string? Description);