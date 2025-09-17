using Microsoft.Extensions.Options;
using Order.Application.Contracts;
using Order.Infrastructure.Options;
using Shared.Domain.Models;
using Stripe;

namespace Order.Infrastructure.Services;

public sealed class StripePaymentService(IOptions<StripeOptions> stripeOptions) : IPaymentService
{
    private readonly StripeClient _stripeClient = new(stripeOptions.Value.SecretKey);
    
    public async Task<Result<string>> PayAsync(decimal amount, string currency, string customerEmail, CancellationToken ct)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency,
                ReceiptEmail = customerEmail,
                PaymentMethodTypes = ["card"]
            };

            PaymentIntentService service = new(_stripeClient);
            PaymentIntent intent = await service.CreateAsync(options, cancellationToken: ct);

            return Result<string>.Success(intent.Id);
        }
        catch (StripeException ex)
        {
            return Result<string>.Failure($"Stripe error: {ex.Message}");
        }
    }
}