namespace Shared.Endpoints;

public static class Roles
{
    public static readonly string[] All = [Seller, Customer, SuperAdmin, Admin, Support]; 
    
    public const string Seller = nameof(Seller);
    
    public const string Customer = nameof(Customer);
    
    public const string SuperAdmin = nameof(SuperAdmin);
    
    public const string Admin = nameof(Admin);
    
    public const string Support = nameof(Support);
}