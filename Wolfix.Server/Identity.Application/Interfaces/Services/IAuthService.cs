using Google.Apis.Auth;
using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<UserRolesDto>> LogInAndGetUserRolesAsync(LogInDto logInDto, CancellationToken ct);
    
    Task<Result<string>> GetTokenByRoleAsync(TokenDto dto, CancellationToken ct);
    
    Task<Result<string>> RegisterAsync(RegisterAsCustomerDto dto, CancellationToken ct);
    
    Task<Result<string>> ChangeEmailAsync(Guid accountId, ChangeEmailDto request, string token, CancellationToken ct);
    
    Task<VoidResult> ChangePasswordAsync(Guid accountId, ChangePasswordDto request, CancellationToken ct);
    
    Task<Result<string>> ContinueWithGoogleAsync(GoogleJsonWebSignature.Payload payload, CancellationToken ct);
}