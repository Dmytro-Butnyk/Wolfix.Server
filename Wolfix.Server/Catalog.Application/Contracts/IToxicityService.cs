using Shared.Domain.Models;

namespace Catalog.Application.Contracts;

public interface IToxicityService
{
    Task<Result<bool>> IsToxic(string text, CancellationToken ct);
}