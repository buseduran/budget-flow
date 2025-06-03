using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Wallets;
using BudgetFlow.Application.Wallets.Commands.CreateWallet;
using BudgetFlow.Application.Wallets.Queries.GetWalletAssets;
using BudgetFlow.Application.Wallets.Queries.GetWalletPagination;
using BudgetFlow.Application.Wallets.Queries.GetWalletUsers;
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

    /// <summary>
    /// Get wallet assets.
    /// </summary>
    /// <param name="getWalletAssets">The wallet ID to get assets for.</param>
    /// <returns>List of assets in the wallet.</returns>
    [HttpGet("Wallet/Assets")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetWalletAssetsResponse>))]
    public async Task<IResult> GetWalletAssetsAsync([FromQuery] GetWalletAssetsQuery getWalletAssets)
    {
        var result = await _mediator.Send(getWalletAssets);
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Get wallet users.
    /// </summary>
    /// <param name="getWalletUsers">The wallet ID to get users for.</param>
    /// <returns>List of users in the wallet.</returns>
    [HttpGet("Wallet/Users")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetWalletUsersResponse>))]
    public async Task<IResult> GetWalletUsersAsync([FromQuery] GetWalletUsersQuery getWalletUsers)
    {
        var result = await _mediator.Send(getWalletUsers);
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }
}
