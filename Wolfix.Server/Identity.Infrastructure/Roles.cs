namespace Identity.Infrastructure;

public static class Roles
{
    public static readonly string[] All = [Seller, Customer]; 
    
    public const string Seller = "Seller";
    
    public const string Customer = "Customer";
}