using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Budget.Commands.CreateEntry;
using BudgetFlow.Application.Budget.Commands.DeleteEntry;
using BudgetFlow.Application.Budget.Commands.UpdateEntry;
using BudgetFlow.Application.Budget.Queries.GetEntryPagination;
using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers
{
    /// <summary>
    /// Controller for managing budget entries
    /// </summary>
    [ApiController]
    [Route("Entry")]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IMediator mediator;
        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for handling requests</param>
        public BudgetController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Creates a new budget entry
        /// </summary>
        /// <param name="createEntryCommand">The command containing entry details</param>
        /// <returns>A boolean indicating whether the creation was successful</returns>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IResult> CreateEntryAsync([FromBody] CreateEntryCommand createEntryCommand)
        {
            var result = await mediator.Send(createEntryCommand);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Updates an existing budget entry
        /// </summary>
        /// <param name="updateEntryCommand">The command containing updated entry details</param>
        /// <returns>A boolean indicating whether the update was successful</returns>
        [HttpPut]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IResult> UpdateEntryAsync([FromBody] UpdateEntryCommand updateEntryCommand)
        {
            var result = await mediator.Send(updateEntryCommand);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Deletes a budget entry by its ID
        /// </summary>
        /// <param name="ID">The ID of the entry to delete</param>
        /// <returns>A boolean indicating whether the deletion was successful</returns>
        [HttpDelete("{ID}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IResult> DeleteEntryAsync([FromRoute] int ID)
        {
            var result = await mediator.Send(new DeleteEntryCommand(ID));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Retrieves a paginated list of budget entries
        /// </summary>
        /// <param name="getEntryPaginationQuery">The query containing pagination parameters</param>
        /// <returns>A paginated list of budget entries</returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<EntryResponse>))]
        public async Task<IResult> GetEntriesPaginationAsync([FromQuery] GetEntryPaginationQuery getEntryPaginationQuery)
        {
            var result = await mediator.Send(getEntryPaginationQuery);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }
    }
}
