using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Models;
using MediatR;

namespace BudgetFlow.Application.User.Commands.CreateUser;
public class CreateUserCommand : IRequest<bool>
{
    public UserModel User { get; set; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository userRepository;

        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await userRepository.CreateAsync(request.User);
            return result;
        }
    }
}

