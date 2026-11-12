
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OzClima.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ArgumentException =>
                StatusCodes.Status400BadRequest,

            KeyNotFoundException =>
                StatusCodes.Status404NotFound,

            _ => StatusCodes.Status500InternalServerError
        };

        var titulo = statusCode switch
        {
            StatusCodes.Status400BadRequest =>
                "Requisição inválida.",

            StatusCodes.Status404NotFound =>
                "Recurso não encontrado.",

            _ => "Erro interno do servidor."
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Erro inesperado na requisição {Path}",
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                "Erro tratado: {Mensagem}",
                exception.Message);
        }

        var mensagem = statusCode ==
            StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado ao processar a requisição."
                : exception.Message;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            Detail = mensagem,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        httpContext.Response.ContentType =
            "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken: cancellationToken);

        return true;
    }
}
