using BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;
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

}

