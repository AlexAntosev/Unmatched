namespace Unmatched.MatchService.Api.Middleware;

using System.Text.Json;

public class ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
{
    private readonly ILogger<RequestLoggingMiddleware> _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception");
            await HandleExceptionAsync(context, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(
            JsonSerializer.Serialize(
                new
                    {
                        error = "An unexpected error occurred.",
                        details = exception.Message
                    }));
    }
}
