using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Auth.Commands.ResetPassword;
public class ResetPasswordCommand : IRequest<Result<bool>>
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly ITokenProvider tokenProvider;
        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenProvider tokenProvider)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.tokenProvider = tokenProvider;
        }

        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return Result.Failure<bool>(UserErrors.InvalidToken);

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure<bool>(UserErrors.EmailCannotBeEmpty);

            if (string.IsNullOrWhiteSpace(request.Password))
                return Result.Failure<bool>(UserErrors.PasswordCannotBeEmpty);

            if (request.Password != request.ConfirmPassword)
                return Result.Failure<bool>(UserErrors.PasswordsDoNotMatch);

            var user = await userRepository.FindByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var isValidToken = tokenProvider.VerifyPasswordResetToken(user.ID, request.Token);
            if (!isValidToken)
                return Result.Failure<bool>(UserErrors.InvalidToken);

            var passwordHash = passwordHasher.Hash(request.Password);
            var result = await userRepository.UpdateAsync(user.Name, user.Email, request.Email, passwordHash);

            return result
                ? Result.Success(true)
                : Result.Failure<bool>(UserErrors.UpdateFailed);
        }
    }
}
