using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Portfolios;
using BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;
using BudgetFlow.Application.Portfolios.Commands.DeletePortfolio;
using BudgetFlow.Application.Portfolios.Commands.UpdatePortfolio;
using BudgetFlow.Application.Portfolios.Queries.GetPortfolio;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<int>))]
    public async Task<IResult> CreatePortfolioAsync([FromBody] CreatePortfolioCommand createPortfolioCommand)
    {
        var result = await mediator.Send(createPortfolioCommand);
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }

    /// <summary>
    /// Deletes a Portfolio. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    public async Task<IResult> DeletePortfolioAsync([FromRoute] int ID)
    {
        var result = await mediator.Send(new DeletePortfolioCommand(ID));
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }

    /// <summary>
    /// Updates a Portfolio. 
    /// </summary>
    /// <param name="updatePortfolioCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    public async Task<IResult> UpdatePortfolioAsync([FromBody] UpdatePortfolioCommand updatePortfolioCommand)
    {
        var result = await mediator.Send(updatePortfolioCommand);
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Portfolios. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<PortfolioResponse>>))]
    public async Task<IResult> GetPortfoliosAsync()
    {
        var result = await mediator.Send(new GetPortfoliosQuery());
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Portfolio. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("{Name}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<PortfolioResponse>))]
    public async Task<IResult> GetPortfolioAsync([FromRoute] string Name)
    {
        var result = await mediator.Send(new GetPortfolioQuery(Name));
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }
}
