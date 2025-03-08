using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Budget.Commands.CreateEntry;
using BudgetFlow.Application.Budget.Commands.DeleteEntry;
using BudgetFlow.Application.Budget.Commands.UpdateEntry;
using BudgetFlow.Application.Budget.Queries.GetEntryPagination;
using BudgetFlow.Application.Budget.Queries.GetGroupedEntries;
using BudgetFlow.Application.Budget.Queries.GetLastFiveEntries;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("Entry")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IMediator mediator;
    public BudgetController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Creates a Budget Entry. 
    /// </summary>
    /// <param name="createEntryCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CreateEntryAsync([FromBody] CreateEntryCommand createEntryCommand)
    {
        return Ok(await mediator.Send(createEntryCommand));
    }
    /// <summary>
    /// Updates a Budget Entry. 
    /// </summary>
    /// <param name="updateEntryCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateEntryAsync([FromBody] UpdateEntryCommand updateEntryCommand)
    {
        return Ok(await mediator.Send(updateEntryCommand));
    }
    /// <summary>
    /// Deletes a Budget Entry. 
    /// </summary>
    /// <param name="deleteEntryCommand"></param>
    /// <returns></returns>
    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> DeleteEntryAsync([FromBody] DeleteEntryCommand deleteEntryCommand)
    {
        return Ok(await mediator.Send(deleteEntryCommand));
    }

    /// <summary>
    /// Pages a Budget Entry. 
    /// </summary>
    /// <param name="getEntryPaginationQuery"></param>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<EntryResponse>))]
    public async Task<IActionResult> GetEntriesPaginationAsync([FromQuery] GetEntryPaginationQuery getEntryPaginationQuery)
    {
        return Ok(await mediator.Send(getEntryPaginationQuery));
    }

    /// <summary>
    /// Get Entries for Dashboard. 
    /// </summary>
    /// <param name="Range"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("Grouped")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((List<EntryResponse> incomes, List<EntryResponse> expenses)))]
    public async Task<IActionResult> GetGroupedEntriesAsync([FromQuery] string Range)
    {
        return Ok(await mediator.Send(new GetGroupedEntriesQuery(Range)));
    }

    /// <summary>
    /// Get Last Entries for Dashboard. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("LastFive")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LastEntryResponse>))]
    public async Task<IActionResult> GetLastFiveEntriesAsync()
    {
        return Ok(await mediator.Send(new GetLastFiveEntriesQuery()));
    }
}