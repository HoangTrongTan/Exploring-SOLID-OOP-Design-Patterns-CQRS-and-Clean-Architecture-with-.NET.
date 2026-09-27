using System.Net;
using System.Text.Json;
using CrudPractice.Application.DTOs;
using CrudPractice.Domain.Exceptions;
using FluentValidation;

namespace CrudPractice.API.Middleware;

/// <summary>
/// [Design Pattern - Chain of Responsibility]
/// Global exception handler - bắt tất cả unhandled exceptions.
/// Mapping:
///   DomainException       → 400 Bad Request
///   ValidationException   → 422 Unprocessable Entity
///   NotFoundException     → 404 Not Found
///   Exception (unexpected)→ 500 Internal Server Error
/// </summary>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex) { await HandleExceptionAsync(context, ex); }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.UnprocessableEntity,
                "Validation failed",
                ve.Errors.Select(e => e.ErrorMessage)),

            DomainException de => (
                HttpStatusCode.BadRequest,
                de.Message,
                Enumerable.Empty<string>()),

            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                nfe.Message,
                Enumerable.Empty<string>()),

            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred",
                Enumerable.Empty<string>())
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception: {Path}", context.Request.Path);
        else
            logger.LogWarning("Handled exception: {Message}", exception.Message);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }
}

/// <summary>
/// [Middleware] Request/Response logging với Correlation ID.
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        context.Response.Headers.Append("X-Correlation-Id", correlationId);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        logger.LogInformation("[{CorrelationId}] {Method} {Path}", correlationId, context.Request.Method, context.Request.Path);

        await next(context);

        sw.Stop();
        logger.LogInformation("[{CorrelationId}] {StatusCode} in {ElapsedMs}ms",
            correlationId, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
