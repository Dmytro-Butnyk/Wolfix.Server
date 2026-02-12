using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Dto.Category.Requests;

public sealed record AddChildCategoryDto(IFormFile Photo, string Name, string? Description,
    IReadOnlyCollection<string> AttributeKeys, IReadOnlyCollection<string> VariantKeys);
    
    //TODO: РЕШИТЬ ОШИБКИ НА СТРАНИЦЕ ПРОДАВЦА + ПРОВЕРИТЬ ЧТО ТАМ ВСЁ ВАЛИДНО ПРИХОДИТ И ЧТО ВСЕ ФОРМЫ ВАЛИДНЫЕ