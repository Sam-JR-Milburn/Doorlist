namespace Doorlist.Presentation.Middleware;

using System.Diagnostics;
using System.Net;
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
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        _logger.LogError(exception, "Unhandled system execution fault trapped. TraceId: {TraceId}", traceId);

        int statusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        
        var errorWrapper = new
        {
            Success = false,
            StatusCode = statusCode,
            Data = (object?)null,
            Error = new {
                message = "A critical system exception occurred.", 
                #if DEBUG
                detail = exception.Message,
                stackTrace = exception.StackTrace
                #endif
            },
            TimeStamp = DateTime.UtcNow.ToString("o"),
            RequestId = traceId
        };
        await httpContext.Response.WriteAsJsonAsync(errorWrapper, cancellationToken);
        return true;
    }
    
}