using System.Net.Http.Json;
using Catalog.Application.Contracts;
using Shared.Domain.Models;

namespace Catalog.Infrastructure.Services;

internal sealed class ToxicityService(HttpClient httpClient) : IToxicityService
{
    public async Task<Result<bool>> IsToxic(string text, CancellationToken ct)
    {
        try
        {
            var payload = new { text };

            var response = await httpClient.PostAsJsonAsync("check", payload, ct);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<bool>(ct);
            
            return Result<bool>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}