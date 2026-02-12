namespace Shared.Core.Dto;

public sealed record CursorPaginationDto<TDto>(IReadOnlyCollection<TDto> Items, Guid? NextCursor);