namespace Identity.Application;

public static class Roles
{
    public static readonly string[] All = [Seller, Customer, Admin, Support]; 
    
    public const string Seller = "Seller";
    
    public const string Customer = "Customer";
    
    public const string Admin = "Admin";
    
    public const string Support = "Support";
}