using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Investments.Queries.GetAssetInvestPagination
{
    public class GetAssetInvestPaginationQuery : IRequest<PaginatedAssetInvestResponse>
    {
        public int Portfolio { get; set; }
        public int Asset { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class GetAssetInvestPaginationQueryHandler : IRequestHandler<GetAssetInvestPaginationQuery, PaginatedAssetInvestResponse>
        {
            private readonly IInvestmentRepository investmentRepository;
            private readonly IHttpContextAccessor httpContextAccessor;
            public GetAssetInvestPaginationQueryHandler(IInvestmentRepository investmentRepository, IHttpContextAccessor httpContextAccessor)
            {
                this.investmentRepository = investmentRepository;
                this.httpContextAccessor = httpContextAccessor;
            }

            public async Task<PaginatedAssetInvestResponse> Handle(GetAssetInvestPaginationQuery request, CancellationToken cancellationToken)
            {
                var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
                var result = await investmentRepository.GetAssetInvestPaginationAsync(userID, request.Portfolio, request.Asset, request.Page, request.PageSize);
                return result;
            }
        }
    }
}
