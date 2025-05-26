using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Wallets.Commands.UpdateCurrency;
public class UpdateCurrencyCommand : IRequest<Result<bool>>
{
    public CurrencyType Currency { get; set; }
    public int WalletID { get; set; }
    public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, Result<bool>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateCurrencyCommandHandler(IWalletRepository walletRepository, ICurrentUserService currentUserService)
        {
            _walletRepository = walletRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result<bool>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var result = await _walletRepository.UpdateCurrencyAsync(request.WalletID, request.Currency);

            return result ? Result.Success(true)
                : Result.Failure<bool>(WalletErrors.UpdateFailed);
        }
    }
}
