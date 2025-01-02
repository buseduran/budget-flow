using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<bool>
    {
        public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IUserRepository _userRepository;
            public LogoutCommandHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
            {
                _httpContextAccessor = httpContextAccessor;
                _userRepository = userRepository;
            }

            public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
            {
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var context = _httpContextAccessor.HttpContext;

                // Clear user identity
                context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());

                _httpContextAccessor.HttpContext.Response.Cookies.Delete("AccessToken");

                GetCurrentUser getCurrentUser = new GetCurrentUser(_httpContextAccessor);
                int userID = getCurrentUser.GetCurrentUserID();
                if (userID == 0)
                {
                    throw new UnauthorizedAccessException();
                }

                var revokeResult = await _userRepository.RevokeToken(userID);
                if (!revokeResult)
                {
                    throw new ApplicationException("Çıkış işlemi başarısız.");
                }

                return true;
            }
        }
    }
}
