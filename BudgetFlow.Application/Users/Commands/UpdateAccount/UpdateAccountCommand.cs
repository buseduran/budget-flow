using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Users.Commands.UpdateAccount;
public class UpdateAccountCommand : IRequest<Result<bool>>
{
    public string Name { get; set; }
    public string OldEmail { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public UpdateAccountCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<bool>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            bool result;
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result = await _userRepository.UpdateWithoutPasswordAsync(request.Name, request.OldEmail, request.Email);
                return result
                     ? Result.Success(true)
                     : Result.Failure<bool>(UserErrors.UpdateFailed);
            }

            request.Password = _passwordHasher.Hash(request.Password);
            result = await _userRepository.UpdateAsync(request.Name, request.OldEmail, request.Email, request.Password);

            return result
                 ? Result.Success(true)
                 : Result.Failure<bool>(UserErrors.UpdateFailed);
        }
    }
}
