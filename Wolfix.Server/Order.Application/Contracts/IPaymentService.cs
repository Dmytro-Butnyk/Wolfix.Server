using Shared.Domain.Models;

namespace Order.Application.Contracts;

public interface IPaymentService
{
    Task<Result<string>> PayAsync(decimal amount, string currency, string customerEmail, CancellationToken ct);
}