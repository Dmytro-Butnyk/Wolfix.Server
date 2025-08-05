namespace Shared.Domain.Interfaces;

public interface IPaginationRepository<TProjection>
{
    Task<int> GetTotalCountAsync(CancellationToken ct);
    Task<IReadOnlyCollection<TProjection>> GetForPageAsync(int page, int pageSize, CancellationToken ct);
}