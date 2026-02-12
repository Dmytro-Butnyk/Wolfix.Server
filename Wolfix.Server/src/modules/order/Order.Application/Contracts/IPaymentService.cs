using Shared.Domain.Models;

namespace Order.Application.Contracts;

public interface IPaymentService<TResponse>
{
    Task<Result<TResponse>> PayAsync(decimal amount, string currency, string customerEmail, CancellationToken ct);
}