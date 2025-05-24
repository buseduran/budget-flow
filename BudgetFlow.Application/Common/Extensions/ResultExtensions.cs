using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace BudgetFlow.Application.Common.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Result is successful, cannot convert to Problem.");
        }

        return HttpResults.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            extensions: new Dictionary<string, object>
            {
                { "error", result.Error ?? BaseErrors.UnknownError }
            }
        );
    }
    public static IResult ToProblemDetails<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Result is successful, cannot convert to Problem.");

        return HttpResults.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            type: "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            extensions: new Dictionary<string, object>
            {
                { "error", result.Error ?? BaseErrors.UnknownError }
            }
        );
    }
}
