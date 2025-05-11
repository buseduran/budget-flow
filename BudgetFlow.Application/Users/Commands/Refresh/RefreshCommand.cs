using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static BudgetFlow.Application.Users.Commands.Refresh.RefreshCommand;

namespace BudgetFlow.Application.Users.Commands.Refresh
{
    public sealed record RefreshCommand : IRequest<Result<Response>>
    {
        public sealed record Response(string AccessToken, string RefreshToken);
        public string RefreshToken { get; set; }

        public class RefreshCommandHandler(
            ITokenProvider tokenProvider,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration) : IRequestHandler<RefreshCommand, Result<Response>>
        {
            public async Task<Result<Response>> Handle(RefreshCommand request, CancellationToken cancellationToken)
            {
                #region Check Token Expiration
                var encodedToken = request.RefreshToken;
                var decodedToken = Uri.UnescapeDataString(encodedToken);

                if (string.IsNullOrEmpty(request.RefreshToken))
                    return Result.Failure<Response>(UserErrors.InvalidRefreshToken);

                var token = await userRepository.GetRefreshTokenAsync(decodedToken);
                if (token is null)
                    return Result.Failure<Response>(UserErrors.InvalidRefreshToken);
                if (token.Expiration < DateTime.UtcNow)
                    return Result.Failure<Response>(UserErrors.RefreshTokenExpired);
                #endregion

                #region Get User By Token
                var user = await userRepository.GetByIdAsync(token.UserID);
                if (user is null)
                    return Result.Failure<Response>(UserErrors.UserNotFound);
                #endregion

                #region Create New Tokens
                var newAccessToken =await tokenProvider.Create(token.User);
                var newRefreshToken = tokenProvider.GenerateRefreshToken();

                token.Token = newRefreshToken;
                token.Expiration = DateTime.UtcNow.AddDays(7);
                var result = await userRepository.UpdateRefreshTokenAsync(token);
                if (!result)
                    return Result.Failure<Response>(UserErrors.RefreshTokenUpdateFailed);

                httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes"))
                });
                httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
                #endregion

                return Result.Success(new Response(newAccessToken, newRefreshToken));
            }
        }
    }
}
