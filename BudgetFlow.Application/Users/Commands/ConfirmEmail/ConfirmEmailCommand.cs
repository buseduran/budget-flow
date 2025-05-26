using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Users.Commands.ConfirmEmail;
public class ConfirmEmailCommand : IRequest<Result<bool>>
{
    public string Token { get; set; }
    public string Email { get; set; }
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<bool>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUserRepository _userRepository;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        public ConfirmEmailCommandHandler(
            ITokenProvider tokenProvider,
            IUserRepository userRepository,
            ITokenBlacklistService tokenBlacklistService)
        {
            _tokenProvider = tokenProvider;
            _userRepository = userRepository;
            _tokenBlacklistService = tokenBlacklistService;
        }
        public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            #region Check token is already used
            if (_tokenBlacklistService.IsBlacklisted(request.Token))
                return Result.Failure<bool>(UserErrors.TokenAlreadyUsed);

            var isValid = _tokenProvider.VerifyEmailConfirmationToken(user.ID, request.Token);
            if (!isValid)
                return Result.Failure<bool>(UserErrors.InvalidToken);

            _tokenBlacklistService.Blacklist(request.Token);
            #endregion

            var result = await _userRepository.ConfirmEmailAsync(request.Email, true);
            if (!result)
                return Result.Failure<bool>(UserErrors.EmailConfirmationFailed);

            return Result.Success(true);
        }
    }
}
