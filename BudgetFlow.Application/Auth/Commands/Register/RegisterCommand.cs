using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.Auth.Commands.Register;
public class RegisterCommand : IRequest<bool>
{
    public UserRegisterModel User { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;

        public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.User.Password != request.User.ConfirmPassword)
            {
                throw new Exception("Passwords do not match");
            }
            User user = new User()
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

