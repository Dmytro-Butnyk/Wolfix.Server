using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Options;

public sealed class JwtOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int Lifetime { get; set; }
    public required string Key { get; set; }
    
    public SymmetricSecurityKey GetKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}