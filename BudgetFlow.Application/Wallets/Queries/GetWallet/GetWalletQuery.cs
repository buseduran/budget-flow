using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Wallets.Queries.GetWalletPagination;
public class GetWalletQuery : IRequest<Result<List<WalletResponse>>>
{
    public class GetWalletQueryHandler : IRequestHandler<GetWalletQuery, Result<List<WalletResponse>>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetWalletQueryHandler(IWalletRepository walletRepository, ICurrentUserService currentUserService)
        {
            _walletRepository = walletRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result<List<WalletResponse>>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
        {
            int userID = _currentUserService.GetCurrentUserID();
            var result = await _walletRepository.GetWalletsAsync(userID);
            return result != null
                ? Result.Success(result)
                : Result.Failure<List<WalletResponse>>(WalletErrors.WalletNotFound);
        }
    }
}
