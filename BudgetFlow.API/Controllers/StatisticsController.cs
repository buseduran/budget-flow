using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Statistics.Queries.GetAnalysisEntries;
using BudgetFlow.Application.Statistics.Queries.GetAssetInvestPagination;
using BudgetFlow.Application.Statistics.Queries.GetAssetRevenue;
using BudgetFlow.Application.Statistics.Queries.GetLastEntries;
using BudgetFlow.Application.Statistics.Queries.GetLastInvestments;
using BudgetFlow.Application.Statistics.Queries.GetPortfolioAssets;
using BudgetFlow.Application.Statistics.Queries.GetWalletContributions;
using BudgetFlow.Application.Statistics.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;

/// <summary>
/// Controller for managing statistics and analytics operations.
/// </summary>
[ApiController]
[Route("[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get Entries for Dashboard. 
    /// </summary>
    /// <param name="getAnalysisEntriesQuery"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("Entries")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnalysisEntriesResponse))]
    public async Task<IResult> GetAnalysisEntriesAsync([FromQuery] GetAnalysisEntriesQuery getAnalysisEntriesQuery)
    {
        var result = await _mediator.Send(getAnalysisEntriesQuery);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Last Entries for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("LatestEntries")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LastEntryResponse>))]
    public async Task<IResult> GetLastEntriesAsync([FromQuery] GetLastEntriesQuery getLastEntriesQuery)
    {
        var result = await _mediator.Send(getLastEntriesQuery);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Last Investments for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("LatestInvestments")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LastEntryResponse>))]
    public async Task<IResult> GetLastInvestmentsAsync([FromQuery] GetLastInvestmentsQuery getLastInvestmentsQuery)
    {
        var result = await _mediator.Send(getLastInvestmentsQuery);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Asset Revenue for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("AssetRevenue/{Portfolio}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetRevenueResponse>))]
    public async Task<IResult> GetAssetRevenueAsync(string Portfolio)
    {
        var result = await _mediator.Send(new GetAssetRevenueQuery(Portfolio));
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Pages an Asset's Investments by Portfolio. 
    /// </summary>
    /// <param name="getAssetInvestPaginationQuery"></param>
    /// <returns></returns>
    [HttpGet("AssetInvestments")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedAssetInvestResponse))]
    public async Task<IResult> GetAssetInvestsPaginationAsync([FromQuery] GetAssetInvestPaginationQuery getAssetInvestPaginationQuery)
    {
        var result = await _mediator.Send(getAssetInvestPaginationQuery);
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Wallet Contributions. 
    /// </summary>
    /// <param name="WalletID"></param>
    /// <returns></returns>
    [HttpGet("Wallet/{WalletID}/Contributions")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WalletContributionResponse>))]
    public async Task<IResult> GetWalletContributions(int WalletID)
    {
        var result = await _mediator.Send(new GetWalletContributionsQuery(WalletID));
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Assets by Portfolio for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Portfolio/{PortfolioID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PortfolioAssetResponse))]
    public async Task<IResult> GetAssetInvestmentsAsync(int PortfolioID)
    {
        var result = await _mediator.Send(new GetPortfolioAssetsQuery(PortfolioID));
        return result.IsSuccess
               ? Results.Ok(result.Value)
               : result.ToProblemDetails();
    }
}