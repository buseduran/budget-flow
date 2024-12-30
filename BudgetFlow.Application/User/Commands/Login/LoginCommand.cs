using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using MediatR;
using static BudgetFlow.Application.User.Commands.Login.LoginCommand;

namespace BudgetFlow.Application.User.Commands.Login;
public sealed record LoginCommand(string Email, string Password) : IRequest<Response>
{
    public sealed record Response(string AccessToken, string RefreshToken);
    public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider, IMapper
         mapper) : IRequestHandler<LoginCommand, Response>
    {
        public async Task<Response> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Şifre hatalı.");
            }

            #region Create Tokens
            var userDto = mapper.Map<UserDto>(user);
            string token = tokenProvider.Create(userDto);

            var refreshToken = new RefreshToken
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

            return new Response(token, refreshToken.Token);
        }
    }
}