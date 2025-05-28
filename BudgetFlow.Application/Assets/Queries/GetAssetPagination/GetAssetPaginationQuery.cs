using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Assets.Queries.GetAssetPagination;
public class GetAssetPaginationQuery : IRequest<Result<PaginatedList<AssetResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public class GetAssetPaginationQueryHandler : IRequestHandler<GetAssetPaginationQuery, Result<PaginatedList<AssetResponse>>>
    {
        private readonly IAssetRepository _assetRepository;

        public GetAssetPaginationQueryHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<Result<PaginatedList<AssetResponse>>> Handle(GetAssetPaginationQuery request, CancellationToken cancellationToken)
        {
            var assetList = await _assetRepository.GetAssetsAsync(request.Page, request.PageSize);

            if (assetList == null)
                return Result.Failure<PaginatedList<AssetResponse>>(AssetErrors.AssetNotFound);

            return Result.Success(assetList);
        }
    }
}
