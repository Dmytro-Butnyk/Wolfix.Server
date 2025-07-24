namespace Wolfix.Domain.Shared.Interfaces;

public interface IPaginationRepository
{
    Task<int> GetTotalCountAsync(CancellationToken ct);
}