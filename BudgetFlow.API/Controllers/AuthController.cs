using BudgetFlow.Application.Auth.Commands.Login;
using BudgetFlow.Application.Auth.Commands.Logout;
using BudgetFlow.Application.Auth.Commands.Register;
using BudgetFlow.Application.Auth.Commands.UpdateAccount;
using BudgetFlow.Application.Budget.Queries.GetLastEntries;
using BudgetFlow.Application.Budget;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Application.Auth.Queries.GetUserCurrency;
using BudgetFlow.Application.Auth.Commands.UpdateUserCurrency;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator mediator;
    public AuthController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="registerCommand"></param>
    /// <returns></returns>
    [HttpPost("Register")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> SignupAsync([FromBody] RegisterCommand registerCommand)
    {
        return Ok(await mediator.Send(registerCommand));
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="loginCommand"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand loginCommand)
    {
        return Ok(await mediator.Send(loginCommand));
    }

    /// <summary>
    /// User logout  
    /// </summary>
    /// <param name="logoutCommand"></param>
    /// <returns></returns>
    [HttpPost("Logout")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [Authorize]
    public async Task<IActionResult> LogoutAsync()
    {
        return Ok(await mediator.Send(new LogoutCommand()));
    }

    /// <summary>
    /// User profile update  
    /// </summary>
    /// <param name="updateAccountCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [Authorize]
    public async Task<IActionResult> UpdateAccountAsync([FromBody] UpdateAccountCommand updateAccountCommand)
    {
        return Ok(await mediator.Send(updateAccountCommand));
    }

    /// <summary>
    /// Get user currency
    /// </summary>
    /// <returns></returns>
    [HttpGet("Currency")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyType))]
    [Authorize]
    public async Task<IActionResult> GetUserCurrencyAsync()
    {
        return Ok(await mediator.Send(new GetUserCurrencyQuery()));
    }

    /// <summary>
    /// User currency update
    /// </summary>
    /// <param name="updateUserCurrencyCommand"></param>
    /// <returns></returns>
    [HttpPut("Currency")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [Authorize]
    public async Task<IActionResult> UpdateCurrencyAsync([FromBody] UpdateUserCurrencyCommand updateUserCurrencyCommand)
    {
        return Ok(await mediator.Send(updateUserCurrencyCommand));
    }
}
