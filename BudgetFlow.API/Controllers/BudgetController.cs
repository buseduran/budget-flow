using BudgetFlow.Application.Budget;
using BudgetFlow.Application.Budget.Commands.CreateEntry;
using BudgetFlow.Application.Budget.Commands.DeleteEntry;
using BudgetFlow.Application.Budget.Commands.UpdateEntry;
using BudgetFlow.Application.Budget.Queries.GetAnalysisEntries;
using BudgetFlow.Application.Budget.Queries.GetEntryPagination;
using BudgetFlow.Application.Budget.Queries.GetLastEntries;
using BudgetFlow.Application.Common.Extensions;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
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
        /// <param name="ID"></param>
        /// <returns></returns>
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
        /// Pages a Budget Entry. 
        /// </summary>
        /// <param name="getEntryPaginationQuery"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get Entries for Dashboard. 
        /// </summary>
        /// <param name="Range"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Analysis")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnalysisEntriesResponse))]
        public async Task<IResult> GetAnalysisEntriesAsync([FromQuery] string Range)
        {
            var result = await mediator.Send(new GetAnalysisEntriesQuery(Range));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }

        /// <summary>
        /// Get Last Entries for Dashboard. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Latest")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LastEntryResponse>))]
        public async Task<IResult> GetLastEntriesAsync()
        {
            var result = await mediator.Send(new GetLastEntriesQuery());
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
        }
    }
}
