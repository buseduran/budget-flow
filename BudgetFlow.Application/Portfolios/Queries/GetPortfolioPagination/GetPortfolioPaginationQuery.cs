using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolioPagination;
public class GetPortfolioPaginationQuery : IRequest<Result<PaginatedList<PortfolioResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int WalletID { get; set; }
    public class GetPortfolioPaginationQueryHandler : IRequestHandler<GetPortfolioPaginationQuery, Result<PaginatedList<PortfolioResponse>>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserWalletRepository userWalletRepository;
        public GetPortfolioPaginationQueryHandler(IPortfolioRepository portfolioRepository, IHttpContextAccessor httpContextAccessor, IUserWalletRepository userWalletRepository)
        {
            this.portfolioRepository = portfolioRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userWalletRepository = userWalletRepository;
        }
        public async Task<Result<PaginatedList<PortfolioResponse>>> Handle(GetPortfolioPaginationQuery request, CancellationToken cancellationToken)
        {
            int userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var userWallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (userWallet == null)
                return Result.Failure<PaginatedList<PortfolioResponse>>(UserWalletErrors.UserWalletNotFound);

            var result = await portfolioRepository.GetPortfoliosAsync(request.Page, request.PageSize, userID, request.WalletID);
            return result != null
                ? Result.Success(result)
                : Result.Failure<PaginatedList<PortfolioResponse>>(PortfolioErrors.PortfolioNotFound);
        }
    }
}
