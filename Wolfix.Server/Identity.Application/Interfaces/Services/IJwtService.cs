namespace Identity.Application.Interfaces.Services;

public interface IJwtService
{
    public string GenerateToken(Guid userId, string email, string role);
}