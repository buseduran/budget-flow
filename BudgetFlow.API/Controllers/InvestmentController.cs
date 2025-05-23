using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Investments;
using BudgetFlow.Application.Investments.Commands.CreateInvestment;
using BudgetFlow.Application.Investments.Commands.DeleteInvestment;
using BudgetFlow.Application.Investments.Commands.UpdateInvestment;
using BudgetFlow.Application.Investments.Queries;
using BudgetFlow.Application.Investments.Queries.GetInvestments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
/// <summary>
/// Controller for managing investment operations.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class InvestmentController : ControllerBase
{
    private readonly IMediator mediator;
    /// <summary>
    /// Initializes a new instance of the <see cref="InvestmentController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    public InvestmentController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    /// <summary>
    /// Creates an Investment. 
    /// </summary>
    /// <param name="createInvestmentCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> CreateInvestmentAsync([FromBody] CreateInvestmentCommand createInvestmentCommand)
    {
        var result = await mediator.Send(createInvestmentCommand);
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Deletes an Investment. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> DeleteInvestmentAsync([FromRoute] int ID)
    {
        var result = await mediator.Send(new DeleteInvestmentCommand(ID));
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Updates an Investment. 
    /// </summary>
    /// <param name="updateInvestmentCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> UpdateInvestmentAsync([FromBody] UpdateInvestmentCommand updateInvestmentCommand)
    {
        var result = await mediator.Send(updateInvestmentCommand);
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Investments. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("{PortfolioID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InvestmentResponse>))]
    public async Task<IResult> GetInvestmentsAsync([FromRoute] int PortfolioID)
    {
        var result = await mediator.Send(new GetInvestmentsQuery(PortfolioID));
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Assets by Portfolio for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Last/{Portfolio}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PortfolioAssetResponse))]
    public async Task<IResult> GetAssetInvestmentsAsync(string Portfolio)
    {
        var result = await mediator.Send(new GetPortfolioAssetsQuery(Portfolio));
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }
}
