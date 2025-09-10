using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
using Shared.Domain.Models;

namespace Identity.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<UserRolesDto>> LogInAndGetUserRolesAsync(LogInDto logInDto);
    
    Task<Result<string>> GetTokenByRoleAsync(TokenDto dto);
    
    Task<Result<string>> RegisterAsCustomerAsync(RegisterAsCustomerDto dto, CancellationToken ct);
    
    Task<Result<string>> RegisterAsSellerAsync(RegisterAsSellerDto dto, CancellationToken ct);
}