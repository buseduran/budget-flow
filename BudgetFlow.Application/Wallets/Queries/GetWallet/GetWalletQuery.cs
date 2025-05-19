using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Wallets.Queries.GetWalletPagination;
public class GetWalletQuery : IRequest<Result<List<WalletResponse>>>
{
    public class GetWalletQueryHandler : IRequestHandler<GetWalletQuery, Result<List<WalletResponse>>>
    {
        private readonly IWalletRepository walletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetWalletQueryHandler(IWalletRepository walletRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.walletRepository = walletRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<List<WalletResponse>>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
        {
            int userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var result = await walletRepository.GetWalletsAsync(userID);
            return result != null
                ? Result.Success(result)
                : Result.Failure<List<WalletResponse>>(WalletErrors.WalletNotFound);
        }
    }
}
