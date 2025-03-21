using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

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
            public GetAssetInvestPaginationQueryHandler(IInvestmentRepository investmentRepository)
            {
                this.investmentRepository = investmentRepository;
            }

            public async Task<PaginatedAssetInvestResponse> Handle(GetAssetInvestPaginationQuery request, CancellationToken cancellationToken)
            {
                var result = await investmentRepository.GetAssetInvestPaginationAsync(request.Portfolio, request.Asset, request.Page, request.PageSize);
                return result;
            }
        }
    }
}
