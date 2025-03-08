using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;

namespace BudgetFlow.Application.Assets.Queries.GetAssetPagination
{
    public class GetAssetPaginationQuery : IRequest<PaginatedList<AssetResponse>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public class GetAssetPaginationQueryHandler : IRequestHandler<GetAssetPaginationQuery, PaginatedList<AssetResponse>>
        {
            private readonly IAssetRepository assetRepository;
            public GetAssetPaginationQueryHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public async Task<PaginatedList<AssetResponse>> Handle(GetAssetPaginationQuery request, CancellationToken cancellationToken)
            {
                var result = await assetRepository.GetPaginatedAsync(request.Page, request.PageSize);
                return result;
            }
        }
    }
}
