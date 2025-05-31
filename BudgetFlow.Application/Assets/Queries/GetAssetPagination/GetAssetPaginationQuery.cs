using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Assets.Queries.GetAssetPagination;
public class GetAssetPaginationQuery : IRequest<Result<PaginatedList<AssetResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string Search { get; set; }
    public AssetType? AssetType { get; set; }

    public class GetAssetPaginationQueryHandler : IRequestHandler<GetAssetPaginationQuery, Result<PaginatedList<AssetResponse>>>
    {
        private readonly IAssetRepository _assetRepository;

        public GetAssetPaginationQueryHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<Result<PaginatedList<AssetResponse>>> Handle(GetAssetPaginationQuery request, CancellationToken cancellationToken)
        {
            var assetList = await _assetRepository.GetAssetsAsync(request.Page, request.PageSize, request.Search, request.AssetType);

            if (assetList == null)
                return Result.Failure<PaginatedList<AssetResponse>>(AssetErrors.AssetNotFound);

            return Result.Success(assetList);
        }
    }
}
