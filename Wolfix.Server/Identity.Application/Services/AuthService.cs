using System.Net;
using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Interfaces.Services;
using Identity.Application.Mapping;
using Identity.Application.Projections;
using Identity.IntegrationEvents;
using Shared.Domain.Models;
using Shared.IntegrationEvents.Interfaces;

namespace Identity.Application.Services;

internal sealed class AuthService(
    IAuthStore authStore,
    IJwtService jwtService,
    IEventBus eventBus) : IAuthService
{
    public async Task<Result<UserRolesDto>> LogInAndGetUserRolesAsync(LogInDto logInDto)
    {
        Result<UserRolesProjection> getUserRolesResult = await authStore.LogInAndGetUserRolesAsync(logInDto.Email, logInDto.Password);

        if (!getUserRolesResult.IsSuccess)
        {
            return Result<UserRolesDto>.Failure(getUserRolesResult.ErrorMessage!, getUserRolesResult.StatusCode);
        }

        UserRolesDto dto = getUserRolesResult.Value!.ToDto();
        return Result<UserRolesDto>.Success(dto);
    }

    public async Task<Result<string>> GetTokenByRoleAsync(TokenDto dto)
    {
        Result<Guid> checkUserExistsAndHasRoleResult = await authStore.CheckUserExistsAndHasRole(dto.Email, dto.Password, dto.Role);

        if (!checkUserExistsAndHasRoleResult.IsSuccess)
        {
            return Result<string>.Failure(checkUserExistsAndHasRoleResult.ErrorMessage!, checkUserExistsAndHasRoleResult.StatusCode);
        }

        string token = jwtService.GenerateToken(checkUserExistsAndHasRoleResult.Value, dto.Email, dto.Role);
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsCustomerAsync(RegisterAsCustomerDto dto, CancellationToken ct)
    {
        Result<Guid> registerResult = await authStore.RegisterAccountAsync(dto.Email, dto.Password, Roles.Customer);

        if (!registerResult.IsSuccess)
        {
            return Result<string>.Failure(registerResult.ErrorMessage!, registerResult.StatusCode);
        }
        
        Guid registeredCustomerId = registerResult.Value;
        
        VoidResult publishResult = await eventBus.PublishAsync(new CustomerAccountCreated
        {
            AccountId = registeredCustomerId
        }, ct);

        if (!publishResult.IsSuccess)
        {
            return Result<string>.Failure(publishResult.ErrorMessage!, publishResult.StatusCode);
        }
        
        string token = jwtService.GenerateToken(registeredCustomerId, dto.Email, Roles.Customer);
        
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsSellerAsync(RegisterAsSellerDto dto, CancellationToken ct)
    {
        Result<Guid> registerResult = await authStore.RegisterAccountAsync(dto.Email, dto.Password, Roles.Seller);

        if (!registerResult.IsSuccess)
        {
            return Result<string>.Failure(registerResult.ErrorMessage!, registerResult.StatusCode);
        }
        
        Guid registeredSellerId = registerResult.Value;

        VoidResult publishResult = await eventBus.PublishAsync(new SellerAccountCreated
        {
            AccountId = registeredSellerId,
            Email = dto.Email,
            Password = dto.Password,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            MiddleName = dto.MiddleName,
            PhoneNumber = dto.PhoneNumber,
            City = dto.City,
            Street = dto.Street,
            HouseNumber = dto.HouseNumber,
            ApartmentNumber = dto.ApartmentNumber,
            BirthDate = dto.BirthDate,
            Document = dto.Document
        }, ct);

        if (!publishResult.IsSuccess)
        {
            return Result<string>.Failure(publishResult.ErrorMessage!, publishResult.StatusCode);
        }
        
        //todo: событие чтобы отправлять документ к админу на рассмотрение
        
        string token = jwtService.GenerateToken(registeredSellerId, dto.Email, Roles.Seller);
        
        return Result<string>.Success(token);
    }
}