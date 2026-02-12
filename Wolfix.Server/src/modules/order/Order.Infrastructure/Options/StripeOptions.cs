namespace Order.Infrastructure.Options;

public sealed class StripeOptions
{
    public required string PublishableKey { get; set; }
    
    public required string SecretKey { get; set; }
    
    public required string WebhookKey { get; set; }
}