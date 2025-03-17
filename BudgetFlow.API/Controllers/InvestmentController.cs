using BudgetFlow.Application.Investments;
using BudgetFlow.Application.Investments.Commands.CreateInvestment;
using BudgetFlow.Application.Investments.Commands.DeleteInvestment;
using BudgetFlow.Application.Investments.Commands.UpdateInvestment;
using BudgetFlow.Application.Investments.Queries.GetAssetInvestments;
using BudgetFlow.Application.Investments.Queries.GetAssetRevenue;
using BudgetFlow.Application.Investments.Queries.GetInvestments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class InvestmentController : ControllerBase
{
    private readonly IMediator mediator;

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
    public async Task<IActionResult> CreateInvestmentAsync([FromBody] CreateInvestmentCommand createInvestmentCommand)
    {
        return Ok(await mediator.Send(createInvestmentCommand));
    }

    /// <summary>
    /// Deletes an Investment. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> DeleteInvestmentAsync([FromRoute] int ID)
    {
        return Ok(await mediator.Send(new DeleteInvestmentCommand(ID)));
    }

    /// <summary>
    /// Updates an Investment. 
    /// </summary>
    /// <param name="updateInvestmentCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateInvestmentAsync([FromBody] UpdateInvestmentCommand updateInvestmentCommand)
    {
        return Ok(await mediator.Send(updateInvestmentCommand));
    }

    /// <summary>
    /// Get Investments. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("{PortfolioID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InvestmentResponse>))]
    public async Task<IActionResult> GetInvestmentsAsync([FromRoute] int PortfolioID)
    {
        return Ok(await mediator.Send(new GetInvestmentsQuery(PortfolioID)));
    }

    /// <summary>
    /// Get Assets by Portfolio for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Last/{Portfolio}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetInvestmentResponse>))]
    public async Task<IActionResult> GetAssetInvestmentsAsync(string Portfolio)
    {
        return Ok(await mediator.Send(new GetAssetInvestmentsQuery(Portfolio)));
    }

    /// <summary>
    /// Get Asset Revenue for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Revenue/{Portfolio}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetRevenueResponse>))]
    public async Task<IActionResult> GetAssetRevenueAsync(string Portfolio)
    {
        return Ok(await mediator.Send(new GetAssetRevenueQuery(Portfolio)));
    }
}
