using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Auth.Commands.Register;
public class RegisterCommand : IRequest<Result<bool>>
{
    public UserRegisterModel User { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<RegisterCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IWalletRepository walletRepository;
        private readonly IPasswordHasher passwordHasher;

        public CreateUserCommandHandler(IUserRepository userRepository, IWalletRepository walletRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.walletRepository = walletRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task<Result<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.User.Password != request.User.ConfirmPassword)
            {
                return Result.Failure<bool>("Passwords do not match");
            }
            User user = new User()
            {
                Name = request.User.Name,
                Email = request.User.Email,
                PasswordHash = passwordHasher.Hash(request.User.Password)
            };
            var userID = await userRepository.CreateAsync(user);
            if (userID != 0)
            {
                Wallet wallet = new Wallet()
                {
                    Balance = 0,
                    UserId = userID,
                    Currency = CurrencyType.USD // default currency is USD
                };
                var result = await walletRepository.CreateWalletAsync(wallet);
                if (!result)
                {
                    return Result.Failure<bool>("Failed to create wallet");
                }
            }
            else
            {
                return Result.Failure<bool>("Failed to create user");
            }

            return Result.Success(true);
        }
    }
}

