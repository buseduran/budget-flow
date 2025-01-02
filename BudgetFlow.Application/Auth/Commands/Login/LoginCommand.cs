using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using static BudgetFlow.Application.Auth.Commands.Login.LoginCommand;

namespace BudgetFlow.Application.Auth.Commands.Login;
public sealed record LoginCommand(string Email, string Password) : IRequest<Response>
{
    public sealed record Response(string AccessToken, string RefreshToken);
    public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider, IMapper
         mapper, IHttpContextAccessor httpContextAccessor) : IRequestHandler<LoginCommand, Response>
    {
        public async Task<Response> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            string accessToken;
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Şifre hatalı.");
            }

            var refreshToken = await userRepository.GetRefreshTokenByUserID(user.ID);
            if (refreshToken is not null)
            {
                var revokeResult = await userRepository.RevokeToken(user.ID);
                //revoke refresh token

                if (!revokeResult)
                {
                    throw new ApplicationException("Çıkış işlemi başarısız.");
                }

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
                var userDto = mapper.Map<UserDto>(user);
                accessToken = tokenProvider.Create(userDto);

                refreshToken = new RefreshToken
                {
                    Token = tokenProvider.GenerateRefreshToken(),
                    UserID = userDto.ID,
                    Expiration = DateTime.UtcNow.AddDays(7)
                };
                var refreshTokenResult = await userRepository.CreateRefreshToken(refreshToken);
                if (!refreshTokenResult)
                {
                    throw new Exception("Token kaydedilemedi.");
                }
                #endregion
            }
            httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15) // Token süresiyle uyumlu
            });
            return new Response(accessToken, refreshToken.Token);
        }
    }
}