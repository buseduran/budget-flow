using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Users.Commands.ResetPassword;
public class ResetPasswordCommand : IRequest<Result<bool>>
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenProvider _tokenProvider;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenProvider tokenProvider,
            ITokenBlacklistService tokenBlacklistService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
            _tokenBlacklistService = tokenBlacklistService;
        }

        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            #region Check inputs
            if (string.IsNullOrWhiteSpace(request.Token))
                return Result.Failure<bool>(UserErrors.InvalidToken);

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure<bool>(UserErrors.EmailCannotBeEmpty);

            if (string.IsNullOrWhiteSpace(request.Password))
                return Result.Failure<bool>(UserErrors.PasswordCannotBeEmpty);

            if (request.Password != request.ConfirmPassword)
                return Result.Failure<bool>(UserErrors.PasswordsDoNotMatch);
            #endregion

            var user = await _userRepository.FindByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            #region Check token is already used
            if (_tokenBlacklistService.IsBlacklisted(request.Token))
                return Result.Failure<bool>(UserErrors.TokenAlreadyUsed);

            if (!_tokenProvider.VerifyPasswordResetToken(user.ID, request.Token))
                return Result.Failure<bool>(UserErrors.InvalidToken);

            _tokenBlacklistService.Blacklist(request.Token);
            #endregion

            #region Update password
            var passwordHash = _passwordHasher.Hash(request.Password);
            var result = await _userRepository.UpdateAsync(user.Name, user.Email, request.Email, passwordHash);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(UserErrors.UpdateFailed);
            #endregion
        }
    }
}
