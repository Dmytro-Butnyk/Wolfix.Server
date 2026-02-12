using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Services;

public sealed class JwtService(IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    
    public string GenerateToken(Guid accountId, Guid profileId, string email, string role)
    {
        ClaimsIdentity identity = GetIdentity(accountId, profileId, email, role);
        
        var timeNow = DateTime.UtcNow;

        JwtSecurityToken token = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            notBefore: timeNow,
            claims: identity.Claims,
            expires: timeNow.Add(TimeSpan.FromDays(_jwtOptions.Lifetime)),
            signingCredentials: new SigningCredentials(_jwtOptions.GetKey(), SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsIdentity GetIdentity(Guid accountId, Guid profileId, string email, string role)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, accountId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("profile_id", profileId.ToString()),
            new(ClaimsIdentity.DefaultRoleClaimType, role)
        ];
        
        ClaimsIdentity identity = new(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );
        
        return identity;
    }
}