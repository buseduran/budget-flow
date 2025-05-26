using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Users.Commands.UpdatePassword;
public class UpdatePasswordCommand : IRequest<Result<bool>>
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;
        public UpdatePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
        }
        public async Task<Result<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var user = await _userRepository.GetByIdAsync(userID);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            if (string.IsNullOrWhiteSpace(request.OldPassword))
                return Result.Failure<bool>(UserErrors.OldPasswordCannotBeEmpty);
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return Result.Failure<bool>(UserErrors.NewPasswordCannotBeEmpty);
            if (request.NewPassword != request.ConfirmNewPassword)
                return Result.Failure<bool>(UserErrors.PasswordsDoNotMatch);


            if (!_passwordHasher.Verify(request.OldPassword, user.PasswordHash))
                return Result.Failure<bool>(UserErrors.InvalidOldPassword);


            var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
            var result = await _userRepository.UpdateAsync(user.Name, user.Email, user.Email, newPasswordHash);
            if (!result)
                return Result.Failure<bool>(UserErrors.UpdateFailed);

            return Result.Success(true);
        }
    }
}
