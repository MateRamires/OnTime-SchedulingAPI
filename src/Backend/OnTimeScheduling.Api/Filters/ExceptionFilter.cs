using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;
using System.Diagnostics;
using System.Net;

namespace OnTimeScheduling.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        if (context.Exception is OnTimeSchedulingException)
        {
            HandleProjectException(context, traceId);
        }
        else
        {
            HandleUnknownException(context, traceId);
        }
    }

    private void HandleProjectException(ExceptionContext context, string traceId)
    {
        switch (context.Exception)
        {
            case ErrorOnValidationException ex:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(new ResponseErrorJson(ex.ErrorsMessages.ToList(), traceId));
                LogAsInformation(context.Exception, traceId);
                break;

            case NotFoundException ex:
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(new ResponseErrorJson(ex.Message, traceId));
                LogAsInformation(context.Exception, traceId);
                break;

            case DomainRuleException ex:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(new ResponseErrorJson(ex.Message, traceId));
                LogAsInformation(context.Exception, traceId);
                break;

            default:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(new ResponseErrorJson("Business rule violation.", traceId));
                LogAsWarning(context.Exception, traceId);
                break;
        }
    }

    private void HandleUnknownException(ExceptionContext context, string traceId)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        context.Result = new ObjectResult(new ResponseErrorJson("Unexpected error occurred.", traceId));

        _logger.LogError(context.Exception, "Unhandled exception. TraceId={TraceId}", traceId);
    }

    private void LogAsInformation(Exception ex, string traceId)
        => _logger.LogInformation(ex, "Handled project exception. TraceId={TraceId}", traceId);

    private void LogAsWarning(Exception ex, string traceId)
        => _logger.LogWarning(ex, "Handled project exception (warning). TraceId={TraceId}", traceId);
}
