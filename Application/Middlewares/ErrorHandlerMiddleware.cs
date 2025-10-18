using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;
using Application.Common.Validations.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;
        IList<ValidationError> validationErrors = [];

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Algumas validações falharam.";
                validationErrors = validationException.Errors.ToList();
                break;
            case BadRequestException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case ConflictException:
                statusCode = HttpStatusCode.Conflict;
                message = exception.Message;
                break;
            case LockedException:
                statusCode = HttpStatusCode.Locked;
                message = exception.Message;
                break;
            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            case ForbiddenException:
                statusCode = HttpStatusCode.Forbidden;
                message = exception.Message;
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "Erro interno no sistema, tente novamente mais tarde.";
                break;
        }

        _logger.LogError("{Exception}", exception);
        context.Response.StatusCode = (int)statusCode;

        if (validationErrors.Any())
        {
            await context.Response.WriteAsync(JsonSerializer.Serialize(validationErrors));
        }
        else
        {
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { statusCode, message }));
        }
    }
}