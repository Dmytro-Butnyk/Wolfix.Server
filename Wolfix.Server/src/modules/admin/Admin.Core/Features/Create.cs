using Admin.Infrastructure;
using Admin.IntegrationEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Core.Endpoints;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using AdminAggregate = Admin.Domain.AdminAggregate.Admin;

namespace Admin.Core.Features;

public static class Create
{
    public sealed record Request(string Email, string Password, string FirstName, string LastName,
        string MiddleName, string PhoneNumber);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("", Handle)
                .RequireAuthorization(AuthorizationRoles.SuperAdmin)
                .WithSummary("Add admin");
        }

        private static async Task<Results<NoContent, BadRequest<string>>> Handle(
            [FromBody] Request request,
            [FromServices] Handler handler,
            CancellationToken ct)
        {
            VoidResult createAdminResult = await handler.HandleAsync(request, ct);
    
            if (createAdminResult.IsFailure)
            {
                return TypedResults.BadRequest(createAdminResult.ErrorMessage);
            }
        
            return TypedResults.NoContent();
        }
    }

    public sealed class Handler(
        AdminContext db,
        EventBus eventBus)
    {
        public async Task<VoidResult> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var @event = new CreateAdmin
            {
                Email = request.Email,
                Password = request.Password
            };
        
            Result<Guid> createAccountResult = await eventBus.PublishWithSingleResultAsync<CreateAdmin, Guid>(@event, cancellationToken);

            if (createAccountResult.IsFailure)
            {
                return VoidResult.Failure(createAccountResult);
            }

            Guid accountId = createAccountResult.Value;

            Result<AdminAggregate> createAdminResult = AdminAggregate.Create(accountId, request.FirstName, request.LastName,
                request.MiddleName, request.PhoneNumber);

            if (createAdminResult.IsFailure)
            {
                return VoidResult.Failure(createAdminResult);
            }

            AdminAggregate admin = createAdminResult.Value!;

            await db.AddAsync(admin, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        
            return VoidResult.Success();
        }
    }
}