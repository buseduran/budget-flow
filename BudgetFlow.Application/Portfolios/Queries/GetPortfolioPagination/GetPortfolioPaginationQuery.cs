using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolioPagination;
public class GetPortfolioPaginationQuery : IRequest<Result<PaginatedList<PortfolioResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int WalletID { get; set; }
    public class GetPortfolioPaginationQueryHandler : IRequestHandler<GetPortfolioPaginationQuery, Result<PaginatedList<PortfolioResponse>>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserWalletRepository _userWalletRepository;
        public GetPortfolioPaginationQueryHandler(
            IPortfolioRepository portfolioRepository,
            ICurrentUserService currentUserService,
            IUserWalletRepository userWalletRepository)
        {
            _portfolioRepository = portfolioRepository;
            _currentUserService = currentUserService;
            _userWalletRepository = userWalletRepository;
        }
        public async Task<Result<PaginatedList<PortfolioResponse>>> Handle(GetPortfolioPaginationQuery request, CancellationToken cancellationToken)
        {
            int userID = _currentUserService.GetCurrentUserID();
            var userWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<PaginatedList<PortfolioResponse>>(UserWalletErrors.UserWalletNotFound);

            var result = await _portfolioRepository.GetPortfoliosAsync(request.Page, request.PageSize, request.WalletID);
            return result != null
                ? Result.Success(result)
                : Result.Failure<PaginatedList<PortfolioResponse>>(PortfolioErrors.PortfolioNotFound);
        }
    }
}
