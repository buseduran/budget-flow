using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Auth.Commands.Logout;
public class LogoutCommand : IRequest<Result<bool>>
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository userRepository;
        public LogoutCommandHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var context = httpContextAccessor.HttpContext;

            // Clear user identity
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());
            httpContextAccessor.HttpContext.Response.Cookies.Delete("AccessToken");

            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();
            if (userID == 0)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var revokeResult = await userRepository.RevokeToken(userID);
            if (!revokeResult)
                return Result.Failure<bool>(UserErrors.LogoutFailed);

            return Result.Success(true);
        }
    }
}
