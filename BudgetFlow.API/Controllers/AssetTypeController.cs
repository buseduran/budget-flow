using BudgetFlow.Application.Asset.Commands.CreateAsset;
using BudgetFlow.Application.AssetType.Commands.CreateAssetType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class AssetTypeController : ControllerBase
{
    private readonly IMediator mediator;
    public AssetTypeController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    /// <summary>
    /// Creates an Asset. 
    /// </summary>
    /// <param name="createAssetTypeCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CreateAssetTypeAsync([FromBody] CreateAssetTypeCommand createAssetTypeCommand)
    {
        return Ok(await mediator.Send(createAssetTypeCommand));
    }


}
