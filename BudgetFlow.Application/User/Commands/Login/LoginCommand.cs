using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.User.Commands.Login;
public class LoginCommand : IRequest<string>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider, IMapper
         mapper) : IRequestHandler<LoginCommand, string>
    {


        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
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
            var userDto = mapper.Map<UserDto>(user);
            string token = tokenProvider.Create(userDto);

            return token;
        }
    }
}
