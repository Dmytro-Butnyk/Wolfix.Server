using Google.Apis.Auth;
using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Mapping;
using Identity.Application.Projections;
using Identity.IntegrationEvents;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using GooglePayload = Google.Apis.Auth.GoogleJsonWebSignature.Payload;

namespace Identity.Application.Services;

public sealed class AuthService(
    IAuthStore authStore,
    JwtService jwtService,
    IConfiguration configuration,
    EventBus eventBus)
{
    private string GooglePassword => configuration["GOOGLE_PASSWORD"] ?? throw new Exception("GOOGLE_PASSWORD is not set");
    
    public async Task<Result<UserRolesDto>> LogInAndGetUserRolesAsync(LogInDto logInDto, CancellationToken ct)
    {
        Result<UserRolesProjection> getUserRolesResult = await authStore.LogInAndGetUserRolesAsync(logInDto.Email, logInDto.Password, ct);

        if (getUserRolesResult.IsFailure)
        {
            return Result<UserRolesDto>.Failure(getUserRolesResult);
        }

        UserRolesDto dto = getUserRolesResult.Value!.ToDto();
        return Result<UserRolesDto>.Success(dto);
    }

    public async Task<Result<string>> GetTokenByRoleAsync(TokenDto dto, CancellationToken ct, string authProvider = "Custom")
    {
        Result<Guid> checkUserExistsAndHasRoleResult = await authStore.CheckUserExistsAndHasRoleAsync(
            dto.Email,
            password: authProvider == "Custom" ? dto.Password : GooglePassword,
            dto.Role,
            ct
        );

        if (checkUserExistsAndHasRoleResult.IsFailure)
        {
            return Result<string>.Failure(checkUserExistsAndHasRoleResult);
        }

        Guid accountId = checkUserExistsAndHasRoleResult.Value;

        Result<Guid> getProfileIdResult = await GetProfileId(accountId, dto.Role, ct);

        if (getProfileIdResult.IsFailure)
        {
            return Result<string>.Failure(getProfileIdResult);
        }
        
        Guid profileId = getProfileIdResult.Value;

        string token = jwtService.GenerateToken(accountId, profileId, dto.Email, dto.Role);
        return Result<string>.Success(token);
    }

    private async Task<Result<Guid>> GetProfileId(Guid accountId, string role, CancellationToken ct)
    {
        if (role == Roles.Customer)
        {
            var @event = new GetCustomerProfileId
            {
                AccountId = accountId
            };
            
            return await eventBus
                .PublishWithSingleResultAsync<GetCustomerProfileId, Guid>(@event, ct);
        }
        
        if (role == Roles.Seller)
        {
            var @event = new GetSellerProfileId
            {
                AccountId = accountId
            };
            
            return await eventBus
                .PublishWithSingleResultAsync<GetSellerProfileId, Guid>(@event, ct);
        }

        if (role == Roles.Admin)
        {
            var @event = new GetAdminProfileId
            {
                AccountId = accountId
            };
            
            return await eventBus
                .PublishWithSingleResultAsync<GetAdminProfileId, Guid>(@event, ct);
        }

        if (role == Roles.SuperAdmin)
        {
            var @event = new GetSuperAdminProfileId
            {
                AccountId = accountId
            };
            
            return await eventBus
                .PublishWithSingleResultAsync<GetSuperAdminProfileId, Guid>(@event, ct);
        }

        if (role == Roles.Support)
        {
            var @event = new GetSupportProfileId
            {
                AccountId = accountId
            };
            
            return await eventBus
                .PublishWithSingleResultAsync<GetSupportProfileId, Guid>(@event, ct);
        }
        
        return Result<Guid>.Failure($"Role {role} not found");
    }

    public async Task<Result<string>> RegisterAsync(RegisterAsCustomerDto dto, CancellationToken ct)
    {
        Result<Guid> registerResult = await authStore.RegisterAccountAsync(dto.Email, dto.Password, Roles.Customer, ct);

        if (registerResult.IsFailure)
        {
            return Result<string>.Failure(registerResult);
        }
        
        Guid registeredCustomerId = registerResult.Value;

        var @event = new CustomerAccountCreated
        {
            AccountId = registeredCustomerId
        };
        
        Result<Guid> createCustomerAndGetIdResult = await eventBus
            .PublishWithSingleResultAsync<CustomerAccountCreated, Guid>(@event, ct);

        if (createCustomerAndGetIdResult.IsFailure)
        {
            return Result<string>.Failure(createCustomerAndGetIdResult);
        }
        
        Guid customerId = createCustomerAndGetIdResult.Value;
        
        string token = jwtService.GenerateToken(registeredCustomerId, customerId, dto.Email, Roles.Customer);
        
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> ChangeEmailAsync(Guid accountId, ChangeEmailDto request, string token, CancellationToken ct)
    {
        VoidResult changeEmailResult = await authStore.ChangeEmailAsync(accountId, request.Email, token, ct);
        
        if (!changeEmailResult.IsSuccess)
        {
            return Result<string>.Failure(changeEmailResult);
        }
        
        return Result<string>.Success(request.Email);
    }

    public async Task<VoidResult> ChangePasswordAsync(Guid accountId, ChangePasswordDto request, CancellationToken ct)
    {
        VoidResult changePasswordResult = await authStore.ChangePasswordAsync(accountId, request.CurrentPassword,
            request.NewPassword, ct);
        
        if (!changePasswordResult.IsSuccess)
        {
            return VoidResult.Failure(changePasswordResult);
        }
        
        return VoidResult.Success();
    }

    public async Task<Result<UserRolesDto>> ContinueWithGoogleAndGetRolesAsync(GooglePayload payload, CancellationToken ct)
    {
        Result<Guid> checkUserExistsResult = await authStore.CheckUserExistsAsync(payload.Email, ct);

        UserRolesDto dto;
        
        if (checkUserExistsResult.IsFailure)
        {
            Result<Guid> registerViaGoogleResult = await RegisterViaGoogle(payload, GooglePassword, ct);

            if (registerViaGoogleResult.IsFailure)
            {
                return Result<UserRolesDto>.Failure(registerViaGoogleResult);
            }
            
            Guid accountId = registerViaGoogleResult.Value;
            
            dto = new UserRolesDto(accountId, payload.Email, [Roles.Customer]);
        }
        else
        {
            Result<UserRolesProjection> getUserRolesResult = await authStore.LogInViaGoogleAndGetUserRolesAsync(payload.Email, GooglePassword, ct);

            if (getUserRolesResult.IsFailure)
            {
                return Result<UserRolesDto>.Failure(getUserRolesResult);
            }

            dto = getUserRolesResult.Value!.ToDto();
        }
        
        return Result<UserRolesDto>.Success(dto);
    }

    private async Task<Result<Guid>> RegisterViaGoogle(GooglePayload payload, string password, CancellationToken ct)
    {
        Result<Guid> registerAccountResult = await authStore.RegisterAccountAsync(
            payload.Email, password, Roles.Customer, ct, "Google");

        if (registerAccountResult.IsFailure)
        {
            return Result<Guid>.Failure(registerAccountResult);
        }
            
        Guid accountId = registerAccountResult.Value;
            
        var @event = new CustomerAccountCreatedViaGoogle
        {
            AccountId = accountId,
            LastName = payload.FamilyName,
            FirstName = payload.GivenName,
            PhotoUrl = payload.Picture
        };
        
        Result<Guid> createCustomerAndGetIdResult = await eventBus
            .PublishWithSingleResultAsync<CustomerAccountCreatedViaGoogle, Guid>(@event, ct);

        if (createCustomerAndGetIdResult.IsFailure)
        {
            return Result<Guid>.Failure(createCustomerAndGetIdResult);
        }
        
        return Result<Guid>.Success(createCustomerAndGetIdResult.Value);
    }
}