using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;

namespace BudgetFlow.Application.Investments.Queries.GetAssetInvestPagination
{
    public class GetAssetInvestPaginationQuery : IRequest<PaginatedList<AssetInvestResponse>>
    {
        public int Portfolio { get; set; }
        public int Asset { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class GetAssetInvestPaginationQueryHandler : IRequestHandler<GetAssetInvestPaginationQuery, PaginatedList<AssetInvestResponse>>
        {
            private readonly IInvestmentRepository investmentRepository;
            public GetAssetInvestPaginationQueryHandler(IInvestmentRepository investmentRepository)
            {
                this.investmentRepository = investmentRepository;
            }

            public async Task<PaginatedList<AssetInvestResponse>> Handle(GetAssetInvestPaginationQuery request, CancellationToken cancellationToken)
            {
                var result = await investmentRepository.GetAssetInvestPaginationAsync(request.Portfolio, request.Asset, request.Page, request.PageSize);
                return result;
            }
        }
    }
}
