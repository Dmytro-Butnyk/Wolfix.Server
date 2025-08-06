namespace Wolfix.Host.Middlewares;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status499ClientClosedRequest,
                ex.Message
            );
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ex.Message
            );
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "text/plain";
        
        await context.Response.WriteAsync(message);
    }
}