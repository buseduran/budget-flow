using BudgetFlow.Application.Assets.Commands.CreateAsset;
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
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CreateAssetAsync([FromBody] CreateAssetCommand createAssetCommand)
    {
        return Ok(await mediator.Send(createAssetCommand));
    }

    /// <summary>
    /// Updates an Asset. 
    /// </summary>
    /// <param name="updateAssetCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateAssetTypeAsync([FromBody] UpdateAssetCommand updateAssetCommand)
    {
        return Ok(await mediator.Send(updateAssetCommand));
    }

}

