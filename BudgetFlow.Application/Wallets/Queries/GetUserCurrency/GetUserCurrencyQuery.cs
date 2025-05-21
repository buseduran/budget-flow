using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Wallets.Queries.GetUserCurrency;
public class GetUserCurrencyQuery : IRequest<Result<CurrencyType>>
{
    public int WalletID { get; set; }
    public class GetUserCurrencyQueryHandler : IRequestHandler<GetUserCurrencyQuery, Result<CurrencyType>>
    {
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetUserCurrencyQueryHandler(
            IUserWalletRepository userWalletRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userWalletRepository = userWalletRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<CurrencyType>> Handle(GetUserCurrencyQuery request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();

            var result = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);

            if (Enum.IsDefined(typeof(CurrencyType), result.Wallet.Currency))
                return Result.Success(result.Wallet.Currency);
            return Result.Success(CurrencyType.USD);
        }
    }
}
