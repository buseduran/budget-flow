using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Queries.GetAssetTypes
{
    public class GetAssetTypesQuery : IRequest<IEnumerable<AssetTypeResponse>>
    {
        public class GetAssetTypesQueryHandler : IRequestHandler<GetAssetTypesQuery, IEnumerable<AssetTypeResponse>>
        {
            private readonly IAssetTypeRepository assetTypeRepository;
            public GetAssetTypesQueryHandler(IAssetTypeRepository assetTypeRepository)
            {
                this.assetTypeRepository = assetTypeRepository;
            }
            public async Task<IEnumerable<AssetTypeResponse>> Handle(GetAssetTypesQuery request, CancellationToken cancellationToken)
            {
                var assetTypes = await assetTypeRepository.GetAssetTypesAsync();
                return assetTypes;
            }
        }
    }
}
