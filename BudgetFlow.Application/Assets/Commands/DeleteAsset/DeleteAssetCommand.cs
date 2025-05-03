using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.DeleteAsset;
public class DeleteAssetCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeleteAssetCommand(int ID)
    {
        this.ID = ID;
    }
    public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, Result<bool>>
    {
        private readonly IAssetRepository assetRepository;
        public DeleteAssetCommandHandler(IAssetRepository assetRepository)
        {
            this.assetRepository = assetRepository;
        }
        public async Task<Result<bool>> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
        {
            if (request.ID <= 0)
                return Result.Failure<bool>(AssetErrors.InvalidAssetId);

            var result = await assetRepository.DeleteAssetAsync(request.ID);
            return result
                ? Result.Success(result)
                : Result.Failure<bool>(AssetErrors.AssetDeletionFailed);
        }
    }
}
