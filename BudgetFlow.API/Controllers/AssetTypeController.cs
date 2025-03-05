using BudgetFlow.Application.AssetTypes.Commands.CreateAssetType;
using BudgetFlow.Application.AssetTypes.Commands.UpdateAssetType;
using BudgetFlow.Application.Budget.Commands.DeleteEntry;
using BudgetFlow.Application.Budget.Commands.UpdateEntry;
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
    /// Creates an Asset Type. 
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

    /// <summary>
    /// Updates a Asset Type. 
    /// </summary>
    /// <param name="updateAssetTypeCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateEntryAsync([FromBody] UpdateAssetTypeCommand updateAssetTypeCommand)
    {
        return Ok(await mediator.Send(updateAssetTypeCommand));
    }



    /// <summary>
    /// Deletes a Budget Entry. 
    /// </summary>
    /// <param name="deleteEntryCommand"></param>
    /// <returns></returns>
    //[HttpDelete]
    //[Produces(MediaTypeNames.Application.Json)]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    //public async Task<IActionResult> DeleteEntryAsync([FromBody] DeleteEntryCommand deleteEntryCommand)
    //{
    //    return Ok(await mediator.Send(deleteEntryCommand));
    //}


}
