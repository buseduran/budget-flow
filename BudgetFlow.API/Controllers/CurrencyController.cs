using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Currencies;
using BudgetFlow.Application.Currencies.Commands.SyncExchangeRates;
using BudgetFlow.Application.Currencies.Queries.GetCurrencies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly IMediator _mediator;

    public CurrencyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all currencies.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CurrencyResponse>))]
    public async Task<IResult> GetCurrencies()
    {
        var result = await _mediator.Send(new GetCurrenciesQuery());
        return result.IsSuccess
             ? Results.Ok(result.Value)
             : result.ToProblemDetails();
    }

    /// <summary>
    /// Synchronize Exchange Rates. 
    /// </summary>
    /// <returns></returns>
    [HttpPost("Sync")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> SyncExchangeRates()
    {
        var result = await _mediator.Send(new SyncExchangeRatesCommand());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
