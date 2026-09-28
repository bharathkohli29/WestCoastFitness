using System.Net;
using System.Text.Json;
using WestCoastFitness.Application.Common.Exceptions;
using FluentValidation;

namespace WestCoastFitness.Api.Middleware;

/// <summary>
/// Centralized exception handler: translates known Application-layer
/// exceptions into safe, structured error responses and logs unexpected
/// exceptions server-side without ever echoing internal details (stack
/// traces, connection strings, SQL) back to the client.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.NotFound, "Resource not found", ex.Message);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage);
            await WriteProblemAsync(context, HttpStatusCode.BadRequest, "Validation failed", string.Join(" ", errors));
        }
        catch (Exception ex)
        {
            // Unexpected exceptions are logged with full detail server-side
            // only; the client only ever sees a generic message.
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred", "Please try again or contact support if the problem persists.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new { title, status = (int)statusCode, detail };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
