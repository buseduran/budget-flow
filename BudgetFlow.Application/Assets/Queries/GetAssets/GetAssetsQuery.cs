using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Assets.Queries.GetAssets
{
    public class GetAssetsQuery : IRequest<Result<List<AssetResponse>>>
    {
        public class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, Result<List<AssetResponse>>>
        {
            private readonly IAssetRepository assetRepository;
            public GetAssetsQueryHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public async Task<Result<List<AssetResponse>>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
            {
                var assetList = await assetRepository.GetAssetsAsync();

                if (assetList == null || assetList.Count == 0)
                {
                    return Result.Failure<List<AssetResponse>>("No Assets found");
                }

                return Result.Success(assetList);
            }
        }
    }
}
