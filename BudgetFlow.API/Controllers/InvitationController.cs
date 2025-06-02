using BudgetFlow.Application.Common.Extensions;
using BudgetFlow.Application.Invitations.Commands.JoinWallet;
using BudgetFlow.Application.Invitations.Commands.SendInvitation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
public class InvitationController : ControllerBase
{
    private readonly IMediator _mediator;
    public InvitationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>  
    /// Create and Send Invitation Link to User.  
    /// </summary>  
    /// <param name="sendInvitationCommand">The command containing invitation details.</param>  
    /// <returns>A result indicating the success or failure of the operation.</returns>  
    [HttpPost("Send")]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> SendInvitationAsync([FromBody] SendInvitationCommand sendInvitationCommand)
    {
        var result = await _mediator.Send(sendInvitationCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>  
    /// Join Wallet by Invitation Link.  
    /// </summary>  
    /// <param name="joinWalletCommand">The command containing wallet join details.</param>  
    /// <returns>A result indicating the success or failure of the operation.</returns>  
    [HttpGet("Join")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> JoinWalletByInvitationAsync([FromQuery] string token)
    {
        var result = await _mediator.Send(new JoinWalletCommand(token));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }
}
