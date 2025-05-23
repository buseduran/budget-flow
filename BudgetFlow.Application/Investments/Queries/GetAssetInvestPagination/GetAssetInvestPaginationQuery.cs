using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Investments.Queries.GetAssetInvestPagination;
public class GetAssetInvestPaginationQuery : IRequest<Result<PaginatedAssetInvestResponse>>
{
    public int PortfolioID { get; set; }
    public int AssetID { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool ConvertToTRY { get; set; } = false;
    public class GetAssetInvestPaginationQueryHandler : IRequestHandler<GetAssetInvestPaginationQuery, Result<PaginatedAssetInvestResponse>>
    {
        private readonly IInvestmentRepository investmentRepository;
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICurrencyRateRepository currencyRateRepository;
        private readonly IUserWalletRepository userWalletRepository;
        public GetAssetInvestPaginationQueryHandler(
            IInvestmentRepository investmentRepository,
            IHttpContextAccessor httpContextAccessor,
            IPortfolioRepository portfolioRepository,
            ICurrencyRateRepository currencyRateRepository,
            IUserWalletRepository userWalletRepository)
        {
            this.investmentRepository = investmentRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.portfolioRepository = portfolioRepository;
            this.currencyRateRepository = currencyRateRepository;
            this.userWalletRepository = userWalletRepository;
        }

        public async Task<Result<PaginatedAssetInvestResponse>> Handle(GetAssetInvestPaginationQuery request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var portfolio = await portfolioRepository.GetPortfolioByIdAsync(request.PortfolioID);

            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(portfolio.WalletID, userID);
            var rate = await currencyRateRepository.GetCurrencyRateByType(userWallet.Wallet.Currency);
            var result = await investmentRepository.GetAssetInvestPaginationAsync(
                portfolio.WalletID, 
                request.PortfolioID, 
                request.AssetID, 
                request.Page, 
                request.PageSize,
                rate.ForexSelling,
                request.ConvertToTRY);

            return result != null
                ? Result.Success(result)
                : Result.Failure<PaginatedAssetInvestResponse>(InvestmentErrors.InvestmentNotFound);
        }
    }
}
