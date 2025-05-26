using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Users;
using BudgetFlow.Application.Users.Commands.ConfirmEmail;
using BudgetFlow.Application.Users.Commands.DeleteUser;
using BudgetFlow.Application.Users.Commands.ForgotPassword;
using BudgetFlow.Application.Users.Commands.Login;
using BudgetFlow.Application.Users.Commands.Logout;
using BudgetFlow.Application.Users.Commands.Refresh;
using BudgetFlow.Application.Users.Commands.Register;
using BudgetFlow.Application.Users.Commands.ResendConfirmEmail;
using BudgetFlow.Application.Users.Commands.ResetPassword;
using BudgetFlow.Application.Users.Commands.UpdateAccount;
using BudgetFlow.Application.Users.Commands.UpdatePassword;
using BudgetFlow.Application.Users.Queries.GetLogPagination;
using BudgetFlow.Application.Users.Queries.GetUserPagination;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
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
        var result = await _mediator.Send(registerCommand);
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
        var result = await _mediator.Send(loginCommand);
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
        var result = await _mediator.Send(refreshCommand);
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
        var result = await _mediator.Send(new LogoutCommand());
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
        var result = await _mediator.Send(updateAccountCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Forgot Password  
    /// </summary>
    /// <param name="forgotPasswordCommand"></param>
    /// <returns></returns>
    [HttpPost("ForgotPassword")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> ForgotPasswordAsync([FromBody] ForgotPasswordCommand forgotPasswordCommand)
    {
        var result = await _mediator.Send(forgotPasswordCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Reset Password  
    /// </summary>
    /// <param name="resetPasswordCommand"></param>
    /// <returns></returns>
    [HttpPost("ResetPassword")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> ResetPasswordAsync([FromBody] ResetPasswordCommand resetPasswordCommand)
    {
        var result = await _mediator.Send(resetPasswordCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Update Password  
    /// </summary>
    /// <param name="updatePasswordCommand"></param>
    /// <returns></returns>
    [HttpPut("UpdatePassword")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> UpdatePasswordAsync([FromBody] UpdatePasswordCommand updatePasswordCommand)
    {
        var result = await _mediator.Send(updatePasswordCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Confirm Email  
    /// </summary>
    /// <param name="confirmEmailCommand"></param>
    /// <returns></returns>
    [HttpGet("ConfirmEmail")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> ConfirmEmailAsync([FromQuery] ConfirmEmailCommand confirmEmailCommand)
    {
        var result = await _mediator.Send(confirmEmailCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Resend Confirm Email  
    /// </summary>
    /// <param name="resendConfirmEmailCommand"></param>
    /// <returns></returns>
    [HttpPost("ResendConfirmEmail")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> ResendConfirmEmailAsync([FromBody] ResendConfirmEmailCommand resendConfirmEmailCommand)
    {
        var result = await _mediator.Send(resendConfirmEmailCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Pages a User Logs
    /// </summary>
    /// <param name="getLogPaginationQuery"></param>
    /// <returns></returns>
    [Authorize(Roles = Role.Admin)]
    [HttpGet("Logs")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<LogResponse>))]
    public async Task<IResult> GetLogsPaginationAsync([FromQuery] GetLogPaginationQuery getLogPaginationQuery)
    {
        var result = await _mediator.Send(getLogPaginationQuery);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Pages a Users
    /// </summary>
    /// <param name="getUserPaginationQuery"></param>
    /// <returns></returns>
    [Authorize(Roles = Role.Admin)]
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<UserPaginationResponse>))]
    public async Task<IResult> GetUsersPaginationAsync([FromQuery] GetUserPaginationQuery getUserPaginationQuery)
    {
        var result = await _mediator.Send(getUserPaginationQuery);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Deletes a User. 
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [Authorize(Roles = Role.Admin)]
    [HttpDelete("{ID}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> DeleteUserAsync([FromRoute] int ID)
    {
        var result = await _mediator.Send(new DeleteUserCommand(ID));
        return result.IsSuccess
                ? Results.Ok(result.Value)
                : result.ToProblemDetails();
    }
}
