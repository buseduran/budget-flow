using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using static BudgetFlow.Application.Auth.Commands.Login.LoginCommand;

namespace BudgetFlow.Application.Auth.Commands.Login;
public sealed record LoginCommand(string Email, string Password) : IRequest<Result<Response>>
{
    public sealed record Response(string AccessToken, string RefreshToken);
    public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider, IMapper
         mapper, IHttpContextAccessor httpContextAccessor) : IRequestHandler<LoginCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            string accessToken;
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<Response>("User not found");
            }

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
                return Result.Failure<Response>("Invalid password");

            var refreshToken = await userRepository.GetRefreshTokenByUserID(user.ID);
            if (refreshToken is not null)
            {
                var revokeResult = await userRepository.RevokeToken(user.ID);
                //revoke refresh token

                if (!revokeResult)
                    return Result.Failure<Response>("Failed to revoke refresh token");

                #region Create Refresh Token
                accessToken = tokenProvider.Create(refreshToken.User);

                refreshToken.ID = Guid.NewGuid();
                refreshToken.Token = tokenProvider.GenerateRefreshToken();
                refreshToken.Expiration = DateTime.UtcNow.AddDays(7);
                await userRepository.CreateRefreshToken(refreshToken);
                #endregion
            }
            else
            {
                #region Create Tokens
                var userDto = mapper.Map<User>(user);
                accessToken = tokenProvider.Create(userDto);

                refreshToken = new RefreshToken
                {
                    Token = tokenProvider.GenerateRefreshToken(),
                    UserID = userDto.ID,
                    Expiration = DateTime.UtcNow.AddDays(7)
                };
                var refreshTokenResult = await userRepository.CreateRefreshToken(refreshToken);
                if (!refreshTokenResult)
                    return Result.Failure<Response>("Failed to create refresh token");
                #endregion
            }
            httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });

            return Result.Success(new Response(accessToken, refreshToken.Token));
        }
    }
}