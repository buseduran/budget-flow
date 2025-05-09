using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Auth.Commands.Register;
public class RegisterCommand : IRequest<Result<bool>>
{
    public UserRegisterModel User { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<RegisterCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;

        public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.User.Password != request.User.ConfirmPassword)
                return Result.Failure<bool>(UserErrors.PasswordsDoNotMatch);

            var userExist = await userRepository.GetByEmailAsync(request.User.Email);
            if (userExist != null)
                return Result.Failure<bool>(UserErrors.UserAlreadyExists);

            User user = new User()
            {
                Name = request.User.Name,
                Email = request.User.Email,
                PasswordHash = passwordHasher.Hash(request.User.Password)
            };
            var userID = await userRepository.CreateAsync(user);
            if (userID is 0)
                return Result.Failure<bool>(UserErrors.CreationFailed);

            return Result.Success(true);
        }
    }
}

