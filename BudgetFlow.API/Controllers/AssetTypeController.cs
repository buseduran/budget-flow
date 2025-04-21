using BudgetFlow.Application.AssetTypes;
using BudgetFlow.Application.AssetTypes.Commands.CreateAssetType;
using BudgetFlow.Application.AssetTypes.Commands.DeleteAssetType;
using BudgetFlow.Application.AssetTypes.Commands.UpdateAssetType;
using BudgetFlow.Application.AssetTypes.Queries.GetAssetTypes;
using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Results;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    public async Task<IResult> CreateAssetTypeAsync([FromBody] CreateAssetTypeCommand createAssetTypeCommand)
    {
        var result = await mediator.Send(createAssetTypeCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Updates an Asset Type. 
    /// </summary>
    /// <param name="updateAssetTypeCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    public async Task<IResult> UpdateAssetTypeAsync([FromBody] UpdateAssetTypeCommand updateAssetTypeCommand)
    {
        var result = await mediator.Send(updateAssetTypeCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Deletes an Asset Type. 
    /// </summary>
    /// <param name="deleteAssetTypeCommand"></param>
    /// <returns></returns>
    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    public async Task<IResult> DeleteAssetTypeAsync([FromBody] DeleteAssetTypeCommand deleteAssetTypeCommand)
    {
        var result = await mediator.Send(deleteAssetTypeCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get List of Asset Types. 
    /// </summary>
    /// <param name="deleteAssetTypeCommand"></param>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<AssetTypeResponse>>))]
    public async Task<IResult> GetAssetTypesAsync()
    {
        var result = await mediator.Send(new GetAssetTypesQuery());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
