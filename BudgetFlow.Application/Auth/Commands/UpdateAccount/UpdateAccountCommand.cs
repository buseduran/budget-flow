using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using MediatR;

namespace BudgetFlow.Application.Auth.Commands.UpdateAccount
{
    public class UpdateAccountCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string OldEmail { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, bool>
        {
            private readonly IUserRepository userRepository;
            private readonly IPasswordHasher passwordHasher;
            public UpdateAccountCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
            {
                this.userRepository = userRepository;
                this.passwordHasher = passwordHasher;
            }

            public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
            {
                request.Password = passwordHasher.Hash(request.Password);
                var result = await userRepository.UpdateAsync(request.Name, request.OldEmail, request.Email, request.Password);

                return result;
            }
        }
    }
}
