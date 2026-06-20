using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Api.ErrorHandling;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = CreateProblem(httpContext, exception);
        LogException(exception, problem.Status ?? StatusCodes.Status500InternalServerError, httpContext.TraceIdentifier);
        await ApiProblemDetails.WriteAsync(httpContext, problem, cancellationToken);
        return true;
    }

    private static ProblemDetails CreateProblem(HttpContext httpContext, Exception exception)
    {
        return exception switch
        {
            ErrorOnValidationException validationException
                => ApiProblemDetails.CreateValidation(httpContext, validationException.ErrorsMessages),

            NotFoundException notFoundException
                => ApiProblemDetails.Create(httpContext, StatusCodes.Status404NotFound, "Not Found", notFoundException.Message),

            ConflictException conflictException
                => ApiProblemDetails.Create(httpContext, StatusCodes.Status409Conflict, "Conflict", conflictException.Message),

            DomainRuleException domainRuleException
                => ApiProblemDetails.Create(httpContext, StatusCodes.Status400BadRequest, "Bad Request", domainRuleException.Message),

            InvalidLoginException invalidLoginException
                => ApiProblemDetails.Create(httpContext, StatusCodes.Status401Unauthorized, "Unauthorized", invalidLoginException.Message),

            ErrorOnUnauthorizedException unauthorizedException
                => ApiProblemDetails.Create(httpContext, StatusCodes.Status403Forbidden, "Forbidden", unauthorizedException.Message),

            OnTimeSchedulingException
                => ApiProblemDetails.Create(httpContext, StatusCodes.Status400BadRequest, "Bad Request", "The requested operation violates a business rule."),

            _ => ApiProblemDetails.Create(httpContext, StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred while processing the request.")
        };
    }

    private void LogException(Exception exception, int statusCode, string traceId)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception. TraceId={TraceId}", traceId);
            return;
        }

        _logger.LogWarning(exception, "Handled API exception. TraceId={TraceId}, Status={Status}", traceId, statusCode);
    }
}
