using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly IMediator mediator;
    public PortfolioController(IMediator mediator)
    {
        this.mediator = mediator;
    }


}

