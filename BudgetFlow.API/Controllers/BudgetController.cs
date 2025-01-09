using BudgetFlow.Application.Budget.Commands.CreateEntry;
using BudgetFlow.Application.Budget.Commands.DeleteEntry;
using BudgetFlow.Application.Budget.Commands.UpdateEntry;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IMediator mediator;
    public BudgetController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Bütçe Hareketi Kaydedilir. 
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
    /// Bütçe Hareketi Güncellenir. 
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
    /// Bütçe Hareketi Silinir. 
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
    /// Bütçe Hareketi Silinir. 
    /// </summary>
    /// <param name="deleteEntryCommand"></param>
    /// <returns></returns>
    //[HttpGet]
    //[Produces(MediaTypeNames.Application.Json)]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    //public async Task<IActionResult> DeleteEntryAsync([FromBody] DeleteEntryCommand deleteEntryCommand)
    //{
    //    return Ok(await mediator.Send(deleteEntryCommand));
    //}

}
