using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Investments;
using BudgetFlow.Application.Wallets.Commands.CreateWallet;
using BudgetFlow.Application.Wallets.Queries.GetWalletPagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;

/// <summary>
/// Controller for managing wallet operations.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalletController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    public WalletController(IMediator mediator)
    {
        _mediator = mediator;
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
        var result = await _mediator.Send(createWalletCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get User's Wallets. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WalletResponse>))]
    public async Task<IResult> GetWalletsAsync()
    {
        var result = await _mediator.Send(new GetWalletQuery());
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }
}
