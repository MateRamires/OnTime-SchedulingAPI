using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace OnTimeScheduling.Api.ErrorHandling;

internal static class ApiProblemDetails
{
    public static ProblemDetails Create(
        HttpContext httpContext,
        int status,
        string title,
        string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = status,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        AddTraceId(problem, httpContext);
        return problem;
    }

    public static ValidationProblemDetails CreateValidation(
        HttpContext httpContext,
        ModelStateDictionary modelState)
    {
        var problem = new ValidationProblemDetails(modelState)
        {
            Type = "about:blank",
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };

        AddTraceId(problem, httpContext);
        return problem;
    }

    public static ValidationProblemDetails CreateValidation(
        HttpContext httpContext,
        IEnumerable<string> errors)
    {
        var distinctErrors = errors
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .Distinct()
            .ToArray();

        var problem = new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            ["general"] = distinctErrors
        })
        {
            Type = "about:blank",
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };

        AddTraceId(problem, httpContext);
        return problem;
    }

    public static async Task WriteAsync(
        HttpContext httpContext,
        ProblemDetails problem,
        CancellationToken ct = default)
    {
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        if (problem is ValidationProblemDetails validationProblem)
        {
            await httpContext.Response.WriteAsJsonAsync(validationProblem, ct);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(problem, ct);
    }

    private static void AddTraceId(ProblemDetails problem, HttpContext httpContext)
    {
        problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
    }
}
