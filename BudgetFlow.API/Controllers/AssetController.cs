﻿using BudgetFlow.Application.Assets;
using BudgetFlow.Application.Assets.Commands.CreateAsset;
using BudgetFlow.Application.Assets.Commands.DeleteAsset;
using BudgetFlow.Application.Assets.Commands.UpdateAsset;
using BudgetFlow.Application.Assets.Queries.GetAssetRate;
using BudgetFlow.Application.Assets.Queries.GetAssets;
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
    /// Creates an Asset. 
    /// </summary>
    /// <param name="createAssetCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CreateAssetAsync([FromForm] CreateAssetCommand createAssetCommand)
    {
        return Ok(await mediator.Send(createAssetCommand));
    }

    /// <summary>
    /// Updates an Asset. 
    /// </summary>
    /// <param name="updateAssetCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Consumes("multipart/form-data")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateAssetTypeAsync([FromForm] UpdateAssetCommand updateAssetCommand)
    {
        return Ok(await mediator.Send(updateAssetCommand));
    }

    /// <summary>
    /// Deletes an Asset. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> DeleteAssetAsync([FromRoute] int ID)
    {
        return Ok(await mediator.Send(new DeleteAssetCommand(ID)));
    }

    /// <summary>
    /// Gets All Assets. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetResponse>))]
    public async Task<IActionResult> GetAssetsPaginationAsync()
    {
        return Ok(await mediator.Send(new GetAssetsQuery()));
    }

    // get rate
    /// <summary>
    /// Gets Asset Rate. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Rate/{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((decimal BuyPrice, decimal SellPrice)))]
    public async Task<IActionResult> GetAssetRateAsync([FromRoute] int ID)
    {
        return Ok(await mediator.Send(new GetAssetRateQuery(ID)));
    }
}
