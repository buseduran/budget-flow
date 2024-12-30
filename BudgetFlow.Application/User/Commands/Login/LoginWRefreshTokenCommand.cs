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
                RefreshToken refreshToken = await userRepository.GetRefreshToken(request.RefreshToken);

                if (refreshToken is null || refreshToken.Expiration < DateTime.UtcNow)
                {
                    throw new ApplicationException("Token süresi doldu.");
                }
                string accessToken = tokenProvider.Create(refreshToken.User);

                refreshToken.ID = Guid.NewGuid();
                refreshToken.Token = tokenProvider.GenerateRefreshToken();
                refreshToken.Expiration = DateTime.UtcNow.AddDays(7);

                await userRepository.CreateRefreshToken(refreshToken);
                return new Response(accessToken, refreshToken.Token);
            }
        }
    }
}
