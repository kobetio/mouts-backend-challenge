using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Global exception-handling middleware. Catches exceptions raised by MediatR handlers (and any
/// other downstream code) and maps them to the standard <see cref="ErrorResponse"/> envelope
/// required by the Sales API error-handling conventions.
/// </summary>
public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, new ErrorResponse
            {
                Type = "ValidationError",
                Error = "Validation failed",
                Detail = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))
            });
        }
        catch (DomainException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, new ErrorResponse
            {
                Type = "ValidationError",
                Error = "Business rule violation",
                Detail = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, new ErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "Resource not found",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, new ErrorResponse
            {
                Type = "Conflict",
                Error = "Operation not allowed",
                Detail = ex.Message
            });
        }
        catch (Exception)
        {
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Type = "InternalServerError",
                Error = "An unexpected error occurred",
                Detail = "An internal server error occurred. Please try again later."
            });
        }
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, ErrorResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
