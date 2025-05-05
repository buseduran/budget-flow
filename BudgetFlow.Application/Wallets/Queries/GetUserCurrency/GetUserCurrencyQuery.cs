using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Wallets.Queries.GetUserCurrency;
public class GetUserCurrencyQuery : IRequest<Result<CurrencyType>>
{
    public class GetUserCurrencyQueryHandler : IRequestHandler<GetUserCurrencyQuery, Result<CurrencyType>>
    {
        private readonly IWalletRepository walletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetUserCurrencyQueryHandler(IWalletRepository walletRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.walletRepository = walletRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<CurrencyType>> Handle(GetUserCurrencyQuery request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var result = await walletRepository.GetUserCurrencyAsync(userID);
            if (Enum.IsDefined(typeof(CurrencyType), result))
                return Result.Success(result);
            return Result.Success(CurrencyType.USD);
        }
    }
}
