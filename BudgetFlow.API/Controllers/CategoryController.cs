using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Categories.Commands.CreateCategory;
using BudgetFlow.Application.Categories.Commands.DeleteCategory;
using BudgetFlow.Application.Categories.Commands.UpdateCategory;
using BudgetFlow.Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly IMediator mediator;
    public CategoryController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create a Category. 
    /// </summary>
    /// <param name="createCategoryCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CreateEntryAsync([FromBody] CreateCategoryCommand createCategoryCommand)
    {
        return Ok(await mediator.Send(createCategoryCommand));
    }

    /// <summary>
    /// Get Categories. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryResponse>))]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        return Ok(await mediator.Send(new GetCategoriesQuery()));
    }

    /// <summary>
    /// Updates a Category. 
    /// </summary>
    /// <param name="updateCategoryCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> UpdateEntryAsync([FromBody] UpdateCategoryCommand updateCategoryCommand)
    {
        return Ok(await mediator.Send(updateCategoryCommand));
    }
    /// <summary>
    /// Deletes a Category. 
    /// </summary>
    /// <param name="deleteEntdeleteCategoryCommandyCommand"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> DeleteEntryAsync([FromRoute] int ID)
    {
        return Ok(await mediator.Send(new DeleteCategoryCommand(ID)));
    }
}

