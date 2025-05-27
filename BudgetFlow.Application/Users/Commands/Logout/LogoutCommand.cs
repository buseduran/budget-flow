using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Users.Commands.Logout;
public class LogoutCommand : IRequest<Result<bool>>
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        public LogoutCommandHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var context = _httpContextAccessor.HttpContext;

            int userID = _currentUserService.GetCurrentUserID();
            if (userID == 0)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            // Clear user identity
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("AccessToken");

           

            var revokeResult = await _userRepository.RevokeTokenAsync(userID);
            if (!revokeResult)
                return Result.Failure<bool>(UserErrors.LogoutFailed);

            return Result.Success(true);
        }
    }
}
