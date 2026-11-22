using System.Net;
using System.Text.Json;
using CatalogService.API.Models;
using CatalogService.Domain.Exceptions;

namespace CatalogService.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            EntityNotFoundException ex => new ErrorResponse((int)HttpStatusCode.NotFound, ex.Message),
            DomainException ex => new ErrorResponse((int)HttpStatusCode.BadRequest, ex.Message),
            ArgumentException ex => new ErrorResponse((int)HttpStatusCode.BadRequest, ex.Message),

            _ => new ErrorResponse(
                (int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred",
                _environment.IsDevelopment() ? exception.ToString() : null)
        };

        response.StatusCode = errorResponse.StatusCode;

        LogException(exception, errorResponse.StatusCode);

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
    }

    private void LogException(Exception exception, int statusCode)
    {
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("A handled exception occurred: {Message}", exception.Message);
        }
    }
}