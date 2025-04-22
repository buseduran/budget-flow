using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Queries.GetAssetTypes;
public class GetAssetTypesQuery : IRequest<Result<List<AssetTypeResponse>>>
{
    public class GetAssetTypesQueryHandler : IRequestHandler<GetAssetTypesQuery, Result<List<AssetTypeResponse>>>
    {
        private readonly IAssetTypeRepository assetTypeRepository;
        public GetAssetTypesQueryHandler(IAssetTypeRepository assetTypeRepository)
        {
            this.assetTypeRepository = assetTypeRepository;
        }
        public async Task<Result<List<AssetTypeResponse>>> Handle(GetAssetTypesQuery request, CancellationToken cancellationToken)
        {
            var assetTypes = await assetTypeRepository.GetAssetTypesAsync();

            if (assetTypes == null)
                return Result.Failure<List<AssetTypeResponse>>("No asset types found");

            return
                Result.Success(assetTypes.ToList());
        }
    }
}
