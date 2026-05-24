namespace Doorlist.Presentation.Middleware;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;

public class ApiResponseFilter : IAsyncResultFilter
{
    public readonly ILogger<ApiResponseFilter> _logger;
    public ApiResponseFilter(ILogger<ApiResponseFilter> logger)
    {
        _logger = logger;
    }
    
    private class ApiResponseWrapper 
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public object? Data { get; set; }
        public object? Error { get; set; }
        public string Timestamp { get; set; } = string.Empty;
        public string? RequestId { get; set; }
    }
    
    /// <summary>
    /// This filter maps API output against the TypeScript ApiResponse<T> type appropriately
    /// </summary>
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context, 
        ResultExecutionDelegate next)
    {
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        int statusCode = context.HttpContext.Response.StatusCode;
        
        // Yield the appropriate wrapper response for the frontend.
        if (context.Result is ObjectResult objectResult && objectResult.Value is not ApiResponseWrapper apiResponse)
        {
            if (statusCode >= 400)
            {
                // Map any validationErrors, replace RFC 7807 .AddProblemDetails()
                string errorMessage = "An unexpected validation error occurred.";
                if (objectResult.Value is SerializableError validationErrors)
                {
                    var firstErrorList = validationErrors.Values.FirstOrDefault() as string[];
                    errorMessage = firstErrorList?.FirstOrDefault() ?? errorMessage;
                } 
                else if (objectResult.Value is string rawText)
                {
                    errorMessage = rawText;
                }
                
                // Failure case
                objectResult.Value = new ApiResponseWrapper
                {
                    Success = false,
                    StatusCode = statusCode,
                    Data = null,
                    Error = new { message = objectResult.Value?.ToString() ?? "An unexpected execution error occurred." },
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    RequestId = traceId
                };
            }
            else
            {
                // Success case
                objectResult.Value = new ApiResponseWrapper
                {
                    Success = true,
                    StatusCode = statusCode,
                    Data = objectResult.Value,
                    Error = null,
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    RequestId = traceId
                };
            }
        }
        
        await next();
    }
}