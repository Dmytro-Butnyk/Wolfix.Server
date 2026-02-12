namespace Shared.Core.Dto;

public sealed record PaginationDto<TDto>(
    int CurrentPage,
    int TotalPages,
    int TotalItems,
    IReadOnlyCollection<TDto> Items)
{
    public static PaginationDto<TDto> Empty(int currentPage)
        => new(currentPage, 0, 0, []);
}