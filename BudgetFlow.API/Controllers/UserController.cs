using BudgetFlow.Application.User.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    private readonly IMediator mediator;
    public UserController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Kullanıcı Kayıt Olur.
    /// </summary>
    /// <param name="createUserCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> SignupAsync([FromBody] CreateUserCommand createUserCommand)
    {
        return Ok(await mediator.Send(createUserCommand));
    }
}

