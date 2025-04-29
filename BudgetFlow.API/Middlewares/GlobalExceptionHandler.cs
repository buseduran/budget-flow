using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BudgetFlow.API.Middlewares;
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            logger.LogError(
                exception,
                "Exception occurred: {Message} {@Exception}",
                exception.Message,
                exception
                );
        }
        else
        {
            logger.LogError(exception, "An unhandled exception occurred while processing the request: {Message}", exception.Message);
        }


        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails);
        return true;
    }
}
