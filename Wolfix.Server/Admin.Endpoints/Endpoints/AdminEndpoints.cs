using Admin.Application.Dto.Requests;
using Admin.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Domain.Models;

namespace Admin.Endpoints.Endpoints;

internal static class AdminEndpoints
{
    private const string Route = "api/admins";
    
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var adminGroup = app.MapGroup(Route)
            .WithTags("Admin");
        
        adminGroup.MapPost("", CreateAdmin)
            .WithSummary("Add admin");
    }

    private static async Task<Results<NoContent, BadRequest<string>>> CreateAdmin(
        [FromBody] CreateAdminDto request,
        [FromServices] IAdminService adminService,
        CancellationToken ct)
    {
        VoidResult createAdminResult = await adminService.CreateAdminAsync(request, ct);
    
        if (createAdminResult.IsFailure)
        {
            return TypedResults.BadRequest(createAdminResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }
}