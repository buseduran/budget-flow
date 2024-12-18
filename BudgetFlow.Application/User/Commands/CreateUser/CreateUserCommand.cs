using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.User.Commands.CreateUser;
public class CreateUserCommand : IRequest<bool>
{
    public UserRegisterModel User { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;

        public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (request.User.Password != request.User.ConfirmPassword)
            {
                throw new Exception("Şifreler uyuşmuyor.");
            }
            UserDto user = new UserDto()
            {
                Name = request.User.Name,
                Email = request.User.Email,
                PasswordHash = passwordHasher.Hash(request.User.Password)
            };

            var result = await userRepository.CreateAsync(user);
            return result;
        }
    }
}

