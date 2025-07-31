namespace Wolfix.Application.Shared.Dto;

public sealed class CursorPaginationDto<TDto>(IReadOnlyCollection<TDto> Items, Guid nextCursor) where TDto : BaseDto;