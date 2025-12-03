using System.Net;
using System.Text.Json;
using FluentValidation;

namespace CleanArcBase.API.Middleware;

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.Errors = validationException.Errors
                    .Select(e => new ErrorDetail(e.PropertyName, e.ErrorMessage))
                    .ToList();
                break;

            case ArgumentException argumentException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argumentException.Message;
                break;

            case InvalidOperationException invalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = invalidOperationException.Message;
                break;

            default:
                _logger.LogError(exception, "An unhandled exception occurred");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An error occurred while processing your request";
                break;
        }

        var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }
}

public record ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public List<ErrorDetail>? Errors { get; set; }
}

public record ErrorDetail(string PropertyName, string Message);
