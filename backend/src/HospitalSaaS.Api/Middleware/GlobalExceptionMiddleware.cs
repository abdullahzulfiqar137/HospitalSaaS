using FluentValidation;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Api.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteValidationErrorAsync(context, exception);
        }
        catch (DomainException exception)
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status409Conflict,
                "BusinessRule.Violation",
                exception.Message
            );
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Request was cancelled or aborted");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled request exception");
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Unexpected",
                "An unexpected error occurred."
            );
        }
    }

    private static Task WriteValidationErrorAsync(
        HttpContext context,
        ValidationException exception
    )
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return context.Response.WriteAsJsonAsync(
            new
            {
                code = "Validation.Failed",
                message = "One or more validation errors occurred.",
                correlationId = context.TraceIdentifier,
                errors = exception
                    .Errors.GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray()
                    ),
            }
        );
    }

    private static Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message
    )
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(
            new
            {
                code,
                message,
                correlationId = context.TraceIdentifier,
            }
        );
    }
}
