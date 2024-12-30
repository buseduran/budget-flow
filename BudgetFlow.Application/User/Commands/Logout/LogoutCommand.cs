using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.User.Commands.Logout
{
    public class LogoutCommand : IRequest<bool>
    {
        public int UserID { get; set; }
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
                GetCurrentUser getCurrentUser = new GetCurrentUser(_httpContextAccessor);
                int userID = getCurrentUser.GetCurrentUserID();
                if (request.UserID != userID)
                {
                    throw new ApplicationException("Kullanıcı bulunamadı.");
                }
                await _userRepository.RevokeToken(userID);

                return true;
            }
        }
    }
}
