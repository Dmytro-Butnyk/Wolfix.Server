using System.Net;
using Admin.Infrastructure;
using Admin.IntegrationEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Endpoints;
using Shared.Core.Exceptions;
using Shared.Domain.Models;
using Shared.IntegrationEvents;
using AdminAggregate = Admin.Domain.AdminAggregate.Admin;

namespace Admin.Core.Features;

public static class Delete
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("{adminId:guid}", Handle)
                .RequireAuthorization(AuthorizationRoles.SuperAdmin)
                .WithSummary("Delete admin");
        }

        private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>, InternalServerError<string>>> Handle(
            [FromRoute] Guid adminId,
            [FromServices] Handler handler,
            CancellationToken cancellationToken)
        {
            VoidResult deleteResult = await handler.HandleAsync(adminId, cancellationToken);

            if (deleteResult.IsFailure)
            {
                return deleteResult.StatusCode switch
                {
                    HttpStatusCode.NotFound => TypedResults.NotFound(deleteResult.ErrorMessage),
                    HttpStatusCode.BadRequest => TypedResults.BadRequest(deleteResult.ErrorMessage),
                    HttpStatusCode.InternalServerError => TypedResults.InternalServerError(deleteResult.ErrorMessage),
                    _ => throw new UnknownStatusCodeException(
                        "AdminEndpoints",
                        nameof(Delete),
                        deleteResult.StatusCode
                    )
                };
            }
        
            return TypedResults.NoContent();
        }
    }

    public sealed class Handler(
        AdminContext db,
        EventBus eventBus)
    {
        public async Task<VoidResult> HandleAsync(Guid adminId, CancellationToken cancellationToken)
        {
            AdminAggregate? admin = await db.Admins.FirstOrDefaultAsync(admin => admin.Id == adminId, cancellationToken);

            if (admin is null)
            {
                return VoidResult.Failure(
                    $"Admin with id: {adminId} not found",
                    HttpStatusCode.NotFound
                );
            }

            var @event = new DeleteAdminAccount
            {
                AccountId = admin.AccountId
            };
        
            VoidResult deleteAccountResult = await eventBus.PublishWithoutResultAsync(@event, cancellationToken);

            if (deleteAccountResult.IsFailure)
            {
                return deleteAccountResult;
            }
        
            db.Remove(admin);
            await db.SaveChangesAsync(cancellationToken);
        
            return VoidResult.Success();
        }
    }
}