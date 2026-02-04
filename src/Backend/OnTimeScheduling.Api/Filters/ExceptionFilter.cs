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

        context.Result = context.Exception switch
        {
            ErrorOnValidationException ex => new BadRequestObjectResult(new ResponseErrorJson(ex.ErrorsMessages.ToList(), traceId)),

            NotFoundException ex => new NotFoundObjectResult(new ResponseErrorJson(ex.Message, traceId)),

            DomainRuleException ex => new BadRequestObjectResult(new ResponseErrorJson(ex.Message, traceId)),

            ErrorOnUnauthorizedException or InvalidLoginException
                => new UnauthorizedObjectResult(new ResponseErrorJson(context.Exception.Message, traceId)),

            OnTimeSchedulingException
                => new BadRequestObjectResult(new ResponseErrorJson("Regra de negócio violada.", traceId)),

            _ => new ObjectResult(new ResponseErrorJson("Erro inesperado no servidor.", traceId))
            { StatusCode = (int)HttpStatusCode.InternalServerError }
        };

        var statusCode = (context.Result as ObjectResult)?.StatusCode ?? 500;
        context.HttpContext.Response.StatusCode = statusCode;

        LogException(context.Exception, traceId, statusCode);
    }

    private void LogException(Exception ex, string traceId, int statusCode)
    {
        if (statusCode >= 500)
            _logger.LogError(ex, "Erro Crítico! TraceId={TraceId}", traceId);
        else if (statusCode == 400)
            _logger.LogInformation(ex, "Aviso de Negócio. TraceId={TraceId}", traceId);
        else
            _logger.LogWarning(ex, "Exceção tratada. TraceId={TraceId}, Status={Status}", traceId, statusCode);
    }
}