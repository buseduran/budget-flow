using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Currencies.Commands.SyncExchangeRates;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
public class CurrencyController : ControllerBase
{
    private readonly IMediator mediator;
    public CurrencyController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Synchronize Exchange Rates. 
    /// </summary>
    /// <returns></returns>
    [HttpPost("Sync")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> SyncExchangeRatesAsync()
    {
        var result = await mediator.Send(new SyncExchangeRatesCommand());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
