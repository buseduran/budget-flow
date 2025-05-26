using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Wallets.Queries.GetUserCurrency;
public class GetUserCurrencyQuery : IRequest<Result<CurrencyType>>
{
    public int WalletID { get; set; }
    public class GetUserCurrencyQueryHandler : IRequestHandler<GetUserCurrencyQuery, Result<CurrencyType>>
    {
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetUserCurrencyQueryHandler(
            IUserWalletRepository userWalletRepository,
            ICurrentUserService currentUserService)
        {
            _userWalletRepository = userWalletRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CurrencyType>> Handle(GetUserCurrencyQuery request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();

            var result = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);

            if (Enum.IsDefined(typeof(CurrencyType), result.Wallet.Currency))
                return Result.Success(result.Wallet.Currency);
            return Result.Success(CurrencyType.USD);
        }
    }
}
