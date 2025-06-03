using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Categories.Commands.CreateCategory;
using BudgetFlow.Application.Categories.Commands.DeleteCategory;
using BudgetFlow.Application.Categories.Commands.UpdateCategory;
using BudgetFlow.Application.Categories.Queries.GetCategoryPagination;
using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Utils;
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
    private readonly IMediator _mediator;
    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a Category. 
    /// </summary>
    /// <param name="createCategoryCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    public async Task<IResult> CreateCategoryAsync([FromBody] CreateCategoryCommand createCategoryCommand)
    {
        var result = await _mediator.Send(createCategoryCommand);
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }

    /// <summary>
    /// Get Categories. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Wallet")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<CategoryResponse>))]
    public async Task<IResult> GetCategoriesAsync([FromQuery] GetCategoryPaginationQuery getCategoryPaginationQuery)
    {
        var result = await _mediator.Send(getCategoryPaginationQuery);
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }

    /// <summary>
    /// Updates a Category. 
    /// </summary>
    /// <param name="updateCategoryCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> UpdateEntryAsync([FromBody] UpdateCategoryCommand updateCategoryCommand)
    {
        var result = await _mediator.Send(updateCategoryCommand);
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }

    /// <summary>
    /// Deletes a Category. 
    /// </summary>
    /// <param name="deleteEntdeleteCategoryCommandyCommand"></param>
    /// <returns></returns>
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> DeleteEntryAsync([FromRoute] int ID)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(ID));
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }
}

