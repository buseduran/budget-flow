using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Assets.Queries.GetAssets
{
    public class GetAssetsQuery : IRequest<List<AssetResponse>>
    {
        public class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, List<AssetResponse>>
        {
            private readonly IAssetRepository assetRepository;
            public GetAssetsQueryHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public async Task<List<AssetResponse>> Handle(GetAssetsQuery request, CancellationToken cancellationToken)
            {
                var result = await assetRepository.GetAssetsAsync();
                return result;
            }
        }
    }
}
