using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shared.Endpoints.Exceptions;

namespace Wolfix.API.ExceptionHandlers;

public sealed class ExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ExceptionHandler> logger
    ) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int statusCode, string title) = exception switch
        {
            OperationCanceledException => (StatusCodes.Status499ClientClosedRequest, "ClientClosedRequest"),
            UnknownStatusCodeException => (StatusCodes.Status500InternalServerError, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, nameof(HttpStatusCode.InternalServerError))
        };

        ProblemDetails problemDetails = new()
        {
            Title = title,
            Status = statusCode,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };
        
        logger.LogError(
            "Title: {Title}, Status: {Status}, Detail: {Detail}, Instance: {Instance}, Exception: {Exception}",
            problemDetails.Title,
            problemDetails.Status,
            problemDetails.Detail,
            problemDetails.Instance,
            exception
        );
        
        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }
}