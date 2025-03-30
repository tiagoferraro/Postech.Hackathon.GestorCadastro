using Microsoft.AspNetCore.Http;
using Postech.Hackathon.GestorCadastro.Api.Models;
using System.Net;
using System.Text.Json;

namespace Postech.Hackathon.GestorCadastro.Api.Settings;

public class ExceptionMiddleware(RequestDelegate _next, ILogger<ExceptionMiddleware> _logger)
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro: {Message}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var errorResponse = new ApiErrorResponse(
            statusCode,
            exception.Message,
            exception.InnerException?.Message!
        );

        var json = JsonSerializer.Serialize(errorResponse, _jsonSerializerOptions);

        await context.Response.WriteAsync(json);
    }
}
