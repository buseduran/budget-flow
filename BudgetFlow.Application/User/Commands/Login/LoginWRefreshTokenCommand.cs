using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using MediatR;
using static BudgetFlow.Application.User.Commands.Login.LoginWRefreshTokenCommand;

namespace BudgetFlow.Application.User.Commands.Login
{
    public sealed record LoginWRefreshTokenCommand(string RefreshToken) : IRequest<Response>
    {
        public sealed record Response(string AccessToken, string RefreshToken);
        public class LoginWRefreshTokenCommandHandler : IRequestHandler<LoginWRefreshTokenCommand, Response>
        {
            private readonly ITokenProvider tokenProvider;
            private readonly IUserRepository userRepository;
            public LoginWRefreshTokenCommandHandler(ITokenProvider tokenProvider, IUserRepository userRepository)
            {
                this.tokenProvider = tokenProvider;
                this.userRepository = userRepository;
            }

            public async Task<Response> Handle(LoginWRefreshTokenCommand request, CancellationToken cancellationToken)
            {

                return null;

            }
        }
    }
}
