﻿using BudgetFlow.Application.User.Commands.Login;
using BudgetFlow.Application.User.Commands.Register;
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
    /// Kullanıcı Giriş Yapar.
    /// </summary>
    /// <param name="loginCommand"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public async Task<IActionResult> SigninAsync([FromBody] LoginCommand loginCommand)
    {
        return Ok(await mediator.Send(loginCommand));
    }
}

