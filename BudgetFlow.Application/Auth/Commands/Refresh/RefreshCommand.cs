using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static BudgetFlow.Application.Auth.Commands.Refresh.RefreshCommand;

namespace BudgetFlow.Application.Auth.Commands.Refresh
{
    public sealed record RefreshCommand : IRequest<Result<Response>>
    {
        public sealed record Response(string AccessToken, string RefreshToken);

        public class RefreshCommandHandler(
            ITokenProvider tokenProvider,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) : IRequestHandler<RefreshCommand, Result<Response>>
        {
            public async Task<Result<Response>> Handle(RefreshCommand request, CancellationToken cancellationToken)
            {
                #region Get UserID
                var UserID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
                #endregion

                #region Check Refresh Token

                var refreshToken = await userRepository.GetRefreshTokenByUserID(UserID);
                if (refreshToken is null)
                    return Result.Failure<Response>(UserErrors.InvalidRefreshToken);
                if (refreshToken.Expiration < DateTime.UtcNow)
                    return Result.Failure<Response>(UserErrors.RefreshTokenExpired);

                #endregion

                #region Check User and Create New Tokens

                var user = await userRepository.GetByIdAsync(refreshToken.UserID);
                if (user is null)
                    return Result.Failure<Response>(UserErrors.UserNotFound);

                var newAccessToken = tokenProvider.Create(refreshToken.User);
                var newRefreshToken = tokenProvider.GenerateRefreshToken();

                #endregion

                #region Update User Tokens

                refreshToken.Token = newRefreshToken;
                refreshToken.Expiration = DateTime.UtcNow.AddDays(7);
                var result = await userRepository.UpdateRefreshToken(refreshToken);
                if (!result)
                    return Result.Failure<Response>(UserErrors.RefreshTokenUpdateFailed);

                httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes"))
                });

                #endregion

                return Result.Success(new Response(newAccessToken, newRefreshToken));
            }
        }
    }
}
