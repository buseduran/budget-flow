using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BudgetFlow.Application.Common.Utils
{
    public class GetCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        public int GetCurrentUserID()
        {
            return int.TryParse(httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int userID)
                ? userID : 0;
        }
    }
}
