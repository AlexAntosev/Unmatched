namespace Unmatched.CatalogService.Api.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
{
    private readonly ILogger<RequestLoggingMiddleware> _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();

    public async Task InvokeAsync(HttpContext context)
    {
        // BEFORE request
        var method = context.Request.Method;
        var path = context.Request.Path;
        var query = context.Request.QueryString.HasValue
            ? context.Request.QueryString.Value
            : "";

        _logger.LogInformation("Incoming request: {Method} {Path}{Query}", method, path, query);

        // Pass the request to the next middleware
        await next(context);

        // AFTER request (optional)
        _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
    }
}