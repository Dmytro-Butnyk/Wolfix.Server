using System.Net;
using Admin.Application.Dto.Requests;
using Admin.Application.Dto.Responses;
using Admin.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Application.Dto;
using Shared.Domain.Models;
using Shared.Endpoints;
using Shared.Endpoints.Exceptions;

namespace Admin.Endpoints.Endpoints;

internal static class AdminEndpoints
{
    private const string Route = "api/admins";
    
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var adminGroup = app.MapGroup(Route)
            .WithTags("Admin");
        
        adminGroup.MapGet("page/{page:int}", GetAllForPage)
            .RequireAuthorization(Roles.SuperAdmin)
            .WithSummary("Get all admins for page");
        
        adminGroup.MapPost("", Create)
            .RequireAuthorization(Roles.SuperAdmin)
            .WithSummary("Add admin");
        
        adminGroup.MapDelete("{adminId:guid}", Delete)
            .RequireAuthorization(Roles.SuperAdmin)
            .WithSummary("Delete admin");
    }

    private static async Task<Ok<PaginationDto<BasicAdminDto>>> GetAllForPage(
        [FromRoute] int page,
        [FromServices] AdminService adminService,
        CancellationToken ct,
        [FromQuery] int pageSize = 50)
    {
        PaginationDto<BasicAdminDto> dto = await adminService.GetForPageAsync(page, pageSize, ct);

        return TypedResults.Ok(dto);
    }

    private static async Task<Results<NoContent, BadRequest<string>>> Create(
        [FromBody] CreateAdminDto request,
        [FromServices] AdminService adminService,
        CancellationToken ct)
    {
        VoidResult createAdminResult = await adminService.CreateAsync(request, ct);
    
        if (createAdminResult.IsFailure)
        {
            return TypedResults.BadRequest(createAdminResult.ErrorMessage);
        }
        
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound<string>, BadRequest<string>, InternalServerError<string>>> Delete(
        [FromRoute] Guid adminId,
        [FromServices] AdminService adminService,
        CancellationToken ct)
    {
        VoidResult deleteResult = await adminService.DeleteAsync(adminId, ct);

        if (deleteResult.IsFailure)
        {
            return deleteResult.StatusCode switch
            {
                HttpStatusCode.NotFound => TypedResults.NotFound(deleteResult.ErrorMessage),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(deleteResult.ErrorMessage),
                HttpStatusCode.InternalServerError => TypedResults.InternalServerError(deleteResult.ErrorMessage),
                _ => throw new UnknownStatusCodeException(nameof(Delete), deleteResult.StatusCode)
            };
        }
        
        return TypedResults.NoContent();
    }
}