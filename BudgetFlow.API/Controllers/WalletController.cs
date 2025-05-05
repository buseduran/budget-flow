using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Wallets.Commands.CreateWallet;
using BudgetFlow.Application.Wallets.Commands.UpdateCurrency;
using BudgetFlow.Application.Wallets.Queries.GetUserCurrency;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IMediator mediator;
    public WalletController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Creates a Wallet. 
    /// </summary>
    /// <param name="createWalletCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> CreateWalletAsync([FromBody] CreateWalletCommand createWalletCommand)
    {
        var result = await mediator.Send(createWalletCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Updates Currency of the Wallet
    /// </summary>
    /// <param name="updateCurrencyCommand"></param>
    /// <returns></returns>
    [HttpPut("Currency")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [Authorize]
    public async Task<IResult> UpdateCurrencyAsync([FromBody] UpdateCurrencyCommand updateCurrencyCommand)
    {
        var result = await mediator.Send(updateCurrencyCommand);
        return result.IsSuccess
        ? Results.Ok(result.Value)
        : result.ToProblemDetails();
    }

    /// <summary>
    /// Get User Currency
    /// </summary>
    /// <returns></returns>
    [HttpGet("Currency")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyType))]
    [Authorize]
    public async Task<IResult> GetUserCurrencyAsync()
    {
        var result = await mediator.Send(new GetUserCurrencyQuery());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
