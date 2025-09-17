using Order.Application.Models;
using Shared.Domain.Models;

namespace Order.Application.Contracts;

public interface IPaymentService
{
    Task<Result<StripePaymentResponse>> PayAsync(decimal amount, string currency, string customerEmail, CancellationToken ct);
}