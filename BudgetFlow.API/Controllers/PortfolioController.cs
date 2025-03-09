using BudgetFlow.Application.Portfolios;
using BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;
using BudgetFlow.Application.Portfolios.Commands.DeletePortfolio;
using BudgetFlow.Application.Portfolios.Commands.UpdatePortfolio;
using BudgetFlow.Application.Portfolios.Queries.GetPortfolios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly IMediator mediator;
    public PortfolioController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Creates a Portfolio. 
    /// </summary>
    /// <param name="createPortfolioCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CreatePortfolioAsync([FromBody] CreatePortfolioCommand createPortfolioCommand)
    {
        return Ok(await mediator.Send(createPortfolioCommand));
    }

    /// <summary>
    /// Deletes a Portfolio. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> DeletePortfolioAsync([FromRoute] int ID)
    {
        return Ok(await mediator.Send(new DeletePortfolioCommand(ID)));
    }

    /// <summary>
    /// Updates a Portfolio. 
    /// </summary>
    /// <param name="updatePortfolioCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdatePortfolioAsync([FromBody] UpdatePortfolioCommand updatePortfolioCommand)
    {
        return Ok(await mediator.Send(updatePortfolioCommand));
    }

    /// <summary>
    /// Get Portfolios. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PortfolioResponse>))]
    public async Task<IActionResult> GetPortfoliosAsync()
    {
        return Ok(await mediator.Send(new GetPortfoliosQuery()));
    }
}
