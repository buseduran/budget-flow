using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Investments.Queries.GetAssetInvestPagination;
public class GetAssetInvestPaginationQuery : IRequest<Result<PaginatedAssetInvestResponse>>
{
    public int Portfolio { get; set; }
    public int Asset { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public class GetAssetInvestPaginationQueryHandler : IRequestHandler<GetAssetInvestPaginationQuery, Result<PaginatedAssetInvestResponse>>
    {
        private readonly IInvestmentRepository investmentRepository;
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetAssetInvestPaginationQueryHandler(
            IInvestmentRepository investmentRepository,
            IHttpContextAccessor httpContextAccessor,
            IPortfolioRepository portfolioRepository)
        {
            this.investmentRepository = investmentRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<Result<PaginatedAssetInvestResponse>> Handle(GetAssetInvestPaginationQuery request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var portfolio = await portfolioRepository.GetPortfolioByIdAsync(request.Portfolio);

            var result = await investmentRepository.GetAssetInvestPaginationAsync(portfolio.WalletID, request.Portfolio, request.Asset, request.Page, request.PageSize);
            return result != null
                ? Result.Success(result)
                : Result.Failure<PaginatedAssetInvestResponse>(InvestmentErrors.InvestmentNotFound);
        }
    }
}
