using Admin.Domain.AdminAggregate.Enums;
using Admin.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Core;
using Shared.Core.Dto;
using Shared.Core.Endpoints;

namespace Admin.Core.Features;

public static class GetAllForPage
{
    public sealed record Response(Guid Id, string FirstName, string LastName, string? MiddleName, string PhoneNumber);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("page/{page:int}", Handle)
                .RequireAuthorization(AuthorizationRoles.SuperAdmin)
                .WithSummary("Get all admins for page");
        }

        private static async Task<Ok<PaginationDto<Response>>> Handle(
            [FromRoute] int page,
            [FromServices] Handler handler,
            CancellationToken ct,
            [FromQuery] int pageSize = 50)
        {
            PaginationDto<Response> dto = await handler.HandeAsync(page, pageSize, ct);

            return TypedResults.Ok(dto);
        }
    }

    public sealed class Handler(AdminContext db)
    {
        public async Task<PaginationDto<Response>> HandeAsync(int page, int pageSize, CancellationToken ct)
        {
            int totalCount = await db.Admins
                .AsNoTracking()
                .Where(admin => admin.Type == AdminType.Basic)
                .CountAsync(ct);

            if (totalCount == 0)
            {
                return PaginationDto<Response>.Empty(page);
            }

            IReadOnlyCollection<Response> dto = await db.Admins
                .AsNoTracking()
                .Where(admin => admin.Type == AdminType.Basic)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(admin => new Response(
                    admin.Id,
                    admin.FullName.FirstName,
                    admin.FullName.LastName,
                    admin.FullName.MiddleName,
                    admin.PhoneNumber.Value
                ))
                .ToListAsync(ct);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
            return new PaginationDto<Response>(page, totalPages, totalCount, dto);
        }
    }
}