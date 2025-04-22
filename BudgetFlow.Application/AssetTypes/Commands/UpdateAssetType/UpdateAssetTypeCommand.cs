using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Commands.UpdateAssetType;
public class UpdateAssetTypeCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public AssetTypeDto AssetType { get; set; }
    public class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand, Result<bool>>
    {
        private readonly IAssetTypeRepository assetTypeRepository;
        public UpdateAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository)
        {
            this.assetTypeRepository = assetTypeRepository;
        }

        public async Task<Result<bool>> Handle(UpdateAssetTypeCommand request, CancellationToken cancellationToken)
        {
            if (request.ID == 0)
                return Result.Failure<bool>("ID cannot be empty");

            var result = await assetTypeRepository.UpdateAssetTypeAsync(request.ID, request.AssetType);
            return result
                ? Result.Success(result)
                : Result.Failure<bool>("Error updating asset type");
        }
    }
}

