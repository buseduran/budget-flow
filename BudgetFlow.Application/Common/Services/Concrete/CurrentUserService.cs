using BudgetFlow.Application.Common.Services.Abstract;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BudgetFlow.Application.Common.Services.Concrete;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentUserID()
    {
        return int.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userID)
       ? userID : 0;
    }
}