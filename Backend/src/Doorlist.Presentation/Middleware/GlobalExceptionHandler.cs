namespace Doorlist.Presentation.Middleware;

using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Intercept all unhandled controller exceptions, return a clean error code.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Intercepts uncaught exceptions across the application pipeline, returning a standard RFC 7807 response with an HTTP 500.
    /// </summary>
    /// <param name="httpContext">Context for this specific HTTP request, including route, user, response stream</param>
    /// <param name="exception">The exception that has specifically occurred</param>
    /// <param name="cancellationToken">Monitor if the request is aborted</param>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Determine cause.
        var requestMethod = httpContext.Request.Method;
        var requestPath = httpContext.Request.Path;
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        _logger.LogError(exception, "An unhandled exception occurred at path {Path} (Method: {Method}). Message: {Message}", requestPath, requestMethod, exception.Message);
        
        // Warn the client.
        var problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Detail = "An unexpected error has occurred on the server."
        };
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}