using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static BudgetFlow.Application.Auth.Commands.Login.LoginCommand;

namespace BudgetFlow.Application.Auth.Commands.Login;
public sealed record LoginCommand(string Email, string Password) : IRequest<Result<Response>>
{
    public sealed record Response(string AccessToken, string RefreshToken);
    public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider, IMapper
         mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : IRequestHandler<LoginCommand, Result<Response>>
    {
        public async Task<Result<Response>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            string accessToken;
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<Response>(UserErrors.UserNotFound);
            }

            #region Check User's Email is Confirmed
            if (!user.IsEmailConfirmed)
            {
                return Result.Failure<Response>(UserErrors.EmailConfirmationFailed);
            }
            #endregion

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
                return Result.Failure<Response>(UserErrors.InvalidPassword);

            var refreshToken = await userRepository.GetRefreshTokenByUserIDAsync(user.ID);
            if (refreshToken is not null)
            {
                var revokeResult = await userRepository.RevokeTokenAsync(user.ID);

                if (!revokeResult)
                    return Result.Failure<Response>(UserErrors.RefreshTokenRevokeFailed);

                #region Create Refresh Token
                accessToken = tokenProvider.Create(refreshToken.User);

                refreshToken.ID = Guid.NewGuid();
                refreshToken.Token = tokenProvider.GenerateRefreshToken();
                refreshToken.Expiration = DateTime.UtcNow.AddDays(7);
                await userRepository.CreateRefreshTokenAsync(refreshToken);
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
                var refreshTokenResult = await userRepository.CreateRefreshTokenAsync(refreshToken);
                if (!refreshTokenResult)
                    return Result.Failure<Response>(UserErrors.RefreshTokenCreationFailed);
                #endregion
            }
            httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes"))
            });
            httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Result.Success(new Response(accessToken, refreshToken.Token));
        }
    }
}