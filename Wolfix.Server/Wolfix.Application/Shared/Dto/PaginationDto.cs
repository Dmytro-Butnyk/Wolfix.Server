namespace Wolfix.Application.Shared.Dto;

public sealed record PaginationDto<TDto>(int CurrentPage, int TotalPages, int TotalItems, IReadOnlyCollection<TDto> Items)
    where TDto : BaseDto;