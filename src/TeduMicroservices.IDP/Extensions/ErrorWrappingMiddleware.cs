using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using TeduMicroservices.IDP.Infrastructure.Common.ApiResult;

namespace TeduMicroservices.IDP.Extensions;

public class ErrorWrappingMiddleware
{
    private readonly ILogger<ErrorWrappingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger)
    {
        _next = next;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        var errorMsg = string.Empty;
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            errorMsg = ex.Message;
            context.Response.StatusCode = 500;
        }

        if (!context.Response.HasStarted && (context.Response.StatusCode == StatusCodes.Status401Unauthorized) ||
            context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiErrorResult<bool>("Unauthorized");
            
            var json = JsonSerializer.Serialize(response);
            
            await context.Response.WriteAsync(json);
        }

        else if (!context.Response.HasStarted && context.Response.StatusCode != StatusCodes.Status204NoContent &&
                 context.Response.StatusCode != StatusCodes.Status202Accepted &&
                 context.Response.StatusCode != StatusCodes.Status200OK &&
                 context.Response.ContentType != "text/html; charset=utf-8")
        {
            context.Response.ContentType = "application/json";
        
            var response = new ApiErrorResult<bool>(errorMsg);
        
            var json = JsonSerializer.Serialize(response);
        
            await context.Response.WriteAsync(json);
        }
    }
}