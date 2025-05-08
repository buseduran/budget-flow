using BudgetFlow.Application.Auth.Commands.ForgotPassword;
using BudgetFlow.Application.Auth.Commands.Login;
using BudgetFlow.Application.Auth.Commands.Logout;
using BudgetFlow.Application.Auth.Commands.Refresh;
using BudgetFlow.Application.Auth.Commands.Register;
using BudgetFlow.Application.Auth.Commands.ResetPassword;
using BudgetFlow.Application.Auth.Commands.UpdateAccount;
using BudgetFlow.Application.Common.Extensions;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IResult> LoginAsync([FromBody] LoginCommand loginCommand)
    {
        var result = await mediator.Send(loginCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Refresh Token
    /// </summary>
    /// <param name="refreshCommand"></param>
    /// <returns></returns>
    [HttpPost("Refresh")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IResult> RefreshAsync([FromBody] RefreshCommand refreshCommand)
    {
        var result = await mediator.Send(refreshCommand);
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [Authorize]
    public async Task<IResult> UpdateAccountAsync([FromBody] UpdateAccountCommand updateAccountCommand)
    {
        var result = await mediator.Send(updateAccountCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Forgot Password  
    /// </summary>
    /// <param name="logoutCommand"></param>
    /// <returns></returns>
    [HttpPost("ForgotPassword")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand forgotPasswordCommand)
    {
        var result = await mediator.Send(forgotPasswordCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Reset Password  
    /// </summary>
    /// <param name="logoutCommand"></param>
    /// <returns></returns>
    [HttpPost("ResetPassword")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> ResetPasswordAsync([FromBody] ResetPasswordCommand resetPasswordCommand)
    {
        var result = await mediator.Send(resetPasswordCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
