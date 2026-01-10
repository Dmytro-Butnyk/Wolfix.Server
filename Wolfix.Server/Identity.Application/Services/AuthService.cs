using Google.Apis.Auth;
using Identity.Application.Dto.Requests;
using Identity.Application.Dto.Responses;
using Identity.Application.Interfaces.Repositories;
using Identity.Application.Mapping;
using Identity.Application.Projections;
using Identity.IntegrationEvents;
using Microsoft.AspNetCore.Identity;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using GooglePayload = Google.Apis.Auth.GoogleJsonWebSignature.Payload;

namespace Identity.Application.Services;

public sealed class AuthService(
    IAuthStore authStore,
    JwtService jwtService,
    EventBus eventBus)
{
    public async Task<Result<UserRolesDto>> LogInAndGetUserRolesAsync(LogInDto logInDto, CancellationToken ct)
    {
        Result<UserRolesProjection> getUserRolesResult = await authStore.LogInAndGetUserRolesAsync(logInDto.Email, logInDto.Password, ct);

        if (!getUserRolesResult.IsSuccess)
        {
            return Result<UserRolesDto>.Failure(getUserRolesResult.ErrorMessage!, getUserRolesResult.StatusCode);
        }

        UserRolesDto dto = getUserRolesResult.Value!.ToDto();
        return Result<UserRolesDto>.Success(dto);
    }

    public async Task<Result<string>> GetTokenByRoleAsync(TokenDto dto, CancellationToken ct)
    {
        Result<Guid> checkUserExistsAndHasRoleResult = await authStore.CheckUserExistsAndHasRoleAsync(dto.Email, dto.Password, dto.Role, ct);

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

    public async Task<Result<string>> ContinueWithGoogleAsync(GooglePayload payload, CancellationToken ct)
    {
        Result<Guid> checkUserExistsResult = await authStore.CheckUserExistsAsync(payload.Email, ct);

        Guid accountId, customerId;
        
        if (checkUserExistsResult.IsFailure)
        {
            Result<(Guid, Guid)> registerViaGoogleResult = await RegisterViaGoogle(payload, ct);

            if (registerViaGoogleResult.IsFailure)
            {
                return Result<string>.Failure(registerViaGoogleResult);
            }

            (accountId, customerId) = registerViaGoogleResult.Value;
        }
        else
        {
            accountId = checkUserExistsResult.Value;

            Result<Guid> logInViaGoogleResult = await LogInViaGoogle(accountId, ct);

            if (logInViaGoogleResult.IsFailure)
            {
                return Result<string>.Failure(logInViaGoogleResult);
            }

            customerId = logInViaGoogleResult.Value;
        }
        
        string token = jwtService.GenerateToken(accountId, customerId, payload.Email, Roles.Customer);
        
        return Result<string>.Success(token);
    }

    private async Task<Result<(Guid, Guid)>> RegisterViaGoogle(GooglePayload payload, CancellationToken ct)
    {
        const string password = "NULL BECAUSE REGISTERED VIA GOOGLE fsjJKI!23";

        Result<Guid> registerAccountResult = await authStore.RegisterAccountAsync(
            payload.Email, password, Roles.Customer, ct, "Google");

        if (registerAccountResult.IsFailure)
        {
            return Result<(Guid, Guid)>.Failure(registerAccountResult);
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
            return Result<(Guid, Guid)>.Failure(createCustomerAndGetIdResult);
        }
        
        Guid customerId = createCustomerAndGetIdResult.Value;
        
        return Result<(Guid, Guid)>.Success((accountId, customerId));
    }

    private async Task<Result<Guid>> LogInViaGoogle(Guid accountId, CancellationToken ct)
    {
        var @event = new GetCustomerProfileId
        {
            AccountId = accountId
        };
            
        Result<Guid> getCustomerProfileIdResult = await eventBus
            .PublishWithSingleResultAsync<GetCustomerProfileId, Guid>(@event, ct);
            
        if (getCustomerProfileIdResult.IsFailure)
        {
            return Result<Guid>.Failure(getCustomerProfileIdResult);
        }
            
        return Result<Guid>.Success(getCustomerProfileIdResult.Value);
    }
}