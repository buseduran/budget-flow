using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Auth.Commands.Register;
public class RegisterCommand : IRequest<bool>
{
    public UserRegisterModel User { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<RegisterCommand, bool>
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
                    throw new Exception("Failed to create wallet.");
                }
            }
            else
            {
                throw new Exception("Failed to create user.");
            }

            return true;
        }
    }
}

