using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Users.Commands.DeleteUser;
public class DeleteUserCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeleteUserCommand(int id)
    {
        ID = id;
    }
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.DeleteAsync(request.ID);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(UserErrors.UserNotFound);
        }
    }
}
