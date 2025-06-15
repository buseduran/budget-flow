using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Assets.Commands.SyncAsset;
using BudgetFlow.Application.Assets.Commands.UpdateAsset;
using BudgetFlow.Application.Assets.Queries.GetAssetPagination;
using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
/// <summary>
/// Controller for managing assets and their operations.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class AssetController : ControllerBase
{
    private readonly IMediator _mediator;
    /// <summary>
    /// Initializes a new instance of the <see cref="AssetController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    public AssetController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Update an Asset. 
    /// </summary>
    /// <param name="updateAssetCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Consumes("multipart/form-data")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> UpdateAssetTypeAsync([FromForm] UpdateAssetCommand updateAssetCommand)
    {
        var result = await _mediator.Send(updateAssetCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Paginated Assets. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<AssetResponse>))]
    public async Task<IResult> GetAssetsPaginationAsync([FromQuery] GetAssetPaginationQuery getAssetPaginationQuery)
    {
        var result = await _mediator.Send(getAssetPaginationQuery);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Synchronize Assets. 
    /// </summary>
    /// <returns></returns>
    [HttpPost("Sync")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> SyncAssetsAsync()
    {
        var result = await _mediator.Send(new SyncAssetCommand());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
