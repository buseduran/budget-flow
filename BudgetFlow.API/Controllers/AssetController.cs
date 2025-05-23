﻿using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Assets.Commands.CreateAsset;
using BudgetFlow.Application.Assets.Commands.DeleteAsset;
using BudgetFlow.Application.Assets.Commands.SyncAsset;
using BudgetFlow.Application.Assets.Commands.UpdateAsset;
using BudgetFlow.Application.Assets.Queries.GetAssetPagination;
using BudgetFlow.Application.Assets.Queries.GetAssetRate;
using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class AssetController : ControllerBase
{
    private readonly IMediator mediator;
    public AssetController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create an Asset. 
    /// </summary>
    /// <param name="createAssetCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> CreateAssetAsync([FromForm] CreateAssetCommand createAssetCommand)
    {
        var result = await mediator.Send(createAssetCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
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
        var result = await mediator.Send(updateAssetCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Delete an Asset. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> DeleteAssetAsync([FromRoute] int ID)
    {
        var result = await mediator.Send(new DeleteAssetCommand(ID));
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
        var result = await mediator.Send(getAssetPaginationQuery);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Asset Rate. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Rate/{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetRateResponse))]
    public async Task<IResult> GetAssetRateAsync([FromRoute] int ID)
    {
        var result = await mediator.Send(new GetAssetRateQuery(ID));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Synchronize Assets. 
    /// </summary>
    /// <param name="syncAssetCommand"></param>
    /// <returns></returns>
    [HttpPost("Sync")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> SyncAssetsAsync([FromBody] SyncAssetCommand syncAssetCommand)
    {
        var result = await mediator.Send(syncAssetCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
