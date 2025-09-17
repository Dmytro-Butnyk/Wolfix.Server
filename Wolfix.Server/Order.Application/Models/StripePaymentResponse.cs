namespace Order.Application.Models;

public sealed record StripePaymentResponse
{
    public required string PaymentIntentId { get; init; }
    
    public required string ClientSecret { get; init; }
}