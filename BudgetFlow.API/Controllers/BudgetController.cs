using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Budget.Commands.CreateEntry;
using BudgetFlow.Application.Budget.Commands.DeleteEntry;
using BudgetFlow.Application.Budget.Commands.UpdateEntry;
using BudgetFlow.Application.Budget.Queries.GetEntryPagination;
using BudgetFlow.Application.Budget.Queries.GetGroupedEntries;
using BudgetFlow.Application.Budget.Queries.GetLastEntries;
using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers
{
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        public async Task<IResult> CreateEntryAsync([FromBody] CreateEntryCommand createEntryCommand)
        {
            var result = await mediator.Send(createEntryCommand);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Updates a Budget Entry. 
        /// </summary>
        /// <param name="updateEntryCommand"></param>
        /// <returns></returns>
        [HttpPut]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        public async Task<IResult> UpdateEntryAsync([FromBody] UpdateEntryCommand updateEntryCommand)
        {
            var result = await mediator.Send(updateEntryCommand);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Deletes a Budget Entry. 
        /// </summary>
        /// <param name="deleteEntryCommand"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
        public async Task<IResult> DeleteEntryAsync([FromBody] DeleteEntryCommand deleteEntryCommand)
        {
            var result = await mediator.Send(deleteEntryCommand);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Pages a Budget Entry. 
        /// </summary>
        /// <param name="getEntryPaginationQuery"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<PaginatedList<EntryResponse>>))]
        public async Task<IResult> GetEntriesPaginationAsync([FromQuery] GetEntryPaginationQuery getEntryPaginationQuery)
        {
            var result = await mediator.Send(getEntryPaginationQuery);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Get Entries for Dashboard. 
        /// </summary>
        /// <param name="Range"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Grouped")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<GroupedEntriesResponse>))]
        public async Task<IResult> GetGroupedEntriesAsync([FromQuery] string Range)
        {
            var result = await mediator.Send(new GetGroupedEntriesQuery(Range));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Get Last Entries for Dashboard. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LastFive")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<LastEntryResponse>>))]
        public async Task<IResult> GetLastEntriesAsync()
        {
            var result = await mediator.Send(new GetLastEntriesQuery());
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }
    }
}
