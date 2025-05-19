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
[Authorize]
public class InvitationController : ControllerBase
{
    private readonly IMediator mediator;
    public InvitationController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Create and Send Invitation Link to User  
    /// </summary>
    /// <param name="sendInvitationCommand"></param>
    /// <returns></returns>
    [HttpPost("Send")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IResult> SendInvitationAsync([FromBody] SendInvitationCommand sendInvitationCommand)
    {
        var result = await mediator.Send(sendInvitationCommand);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.ToProblemDetails();
    }

    /// <summary>
    /// Join Wallet by Invitation Link
    /// </summary>
    /// <param name="joinWalletCommand"></param>
    /// <returns></returns>
    //[HttpPost("Join")]
    //[Produces(MediaTypeNames.Application.Json)]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    //public async Task<IResult> JoinWalletByInvitationAsync([FromBody] JoinWalletCommand joinWalletCommand)
    //{
    //    var result = await mediator.Send(joinWalletCommand);
    //    return result.IsSuccess
    //        ? Results.Ok(result.Value)
    //        : result.ToProblemDetails();
    //}

}
