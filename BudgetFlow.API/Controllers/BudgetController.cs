using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Budget.Commands.CreateEntry;
using BudgetFlow.Application.Budget.Commands.DeleteEntry;
using BudgetFlow.Application.Budget.Commands.UpdateEntry;
using BudgetFlow.Application.Budget.Queries.GetEntryPagination;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
//[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IMediator mediator;
    public BudgetController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Recordes a Budget Entry. 
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
}
