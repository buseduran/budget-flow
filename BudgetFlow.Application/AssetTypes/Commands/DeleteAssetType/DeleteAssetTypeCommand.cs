using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Commands.DeleteAssetType
{
    public class DeleteAssetTypeCommand : IRequest<Result<bool>>
    {
        public int ID { get; set; }
        public DeleteAssetTypeCommand(int ID)
        {
            this.ID = ID;
        }

        public class DeleteAssetTypeCommandHandler : IRequestHandler<DeleteAssetTypeCommand, Result<bool>>
        {
            private readonly IAssetTypeRepository assetTypeRepository;
            public DeleteAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository)
            {
                this.assetTypeRepository = assetTypeRepository;
            }
            public async Task<Result<bool>> Handle(DeleteAssetTypeCommand request, CancellationToken cancellationToken)
            {
                if (request.ID == 0)
                    return
                        Result.Failure<bool>("ID cannot be 0");

                var deleted = await assetTypeRepository.DeleteAssetTypeAsync(request.ID);

                return deleted
                    ? Result.Success(true)
                    : Result.Failure<bool>("Failed to delete Asset Type");
            }
        }
    }
}
