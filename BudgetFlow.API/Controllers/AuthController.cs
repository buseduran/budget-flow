using BudgetFlow.Application.Auth.Commands.Login;
using BudgetFlow.Application.Auth.Commands.Logout;
using BudgetFlow.Application.Auth.Commands.Register;
using BudgetFlow.Application.Auth.Commands.UpdateAccount;
using BudgetFlow.Application.Auth.Commands.UpdateUserCurrency;
using BudgetFlow.Application.Auth.Queries.GetUserCurrency;
using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    public async Task<IResult> SignupAsync([FromBody] RegisterCommand registerCommand)
    {
        var result = await mediator.Send(registerCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="loginCommand"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<string>))]
    public async Task<IResult> LoginAsync([FromBody] LoginCommand loginCommand)
    {
        var result = await mediator.Send(loginCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// User logout  
    /// </summary>
    /// <param name="logoutCommand"></param>
    /// <returns></returns>
    [HttpPost("Logout")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    [Authorize]
    public async Task<IResult> LogoutAsync()
    {
        var result = await mediator.Send(new LogoutCommand());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// User profile update  
    /// </summary>
    /// <param name="updateAccountCommand"></param>
    /// <returns></returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<bool>))]
    [Authorize]
    public async Task<IResult> UpdateAccountAsync([FromBody] UpdateAccountCommand updateAccountCommand)
    {
        var result = await mediator.Send(updateAccountCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Get user currency
    /// </summary>
    /// <returns></returns>
    [HttpGet("Currency")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CurrencyType>))]
    [Authorize]
    public async Task<IResult> GetUserCurrencyAsync()
    {
        var result = await mediator.Send(new GetUserCurrencyQuery());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// User currency update
    /// </summary>
    /// <param name="updateUserCurrencyCommand"></param>
    /// <returns></returns>
    [HttpPut("Currency")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<string>))]
    [Authorize]
    public async Task<IResult> UpdateCurrencyAsync([FromBody] UpdateUserCurrencyCommand updateUserCurrencyCommand)
    {
        var result = await mediator.Send(updateUserCurrencyCommand);
        return result.IsSuccess
        ? Results.Ok(result.Value)
        : result.ToProblemDetails();
    }
}
