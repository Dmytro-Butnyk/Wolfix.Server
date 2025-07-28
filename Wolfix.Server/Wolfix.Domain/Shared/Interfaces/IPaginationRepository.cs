namespace Wolfix.Domain.Shared.Interfaces;

public interface IPaginationRepository<TProjection>
{
    Task<int> GetTotalCountAsync(CancellationToken ct);
    Task<IEnumerable<TProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct);
}