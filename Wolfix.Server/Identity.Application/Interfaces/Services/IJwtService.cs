namespace Identity.Application.Interfaces.Services;

public interface IJwtService
{
    public string GenerateToken(Guid accountId, Guid profileId, string email, string role);
}