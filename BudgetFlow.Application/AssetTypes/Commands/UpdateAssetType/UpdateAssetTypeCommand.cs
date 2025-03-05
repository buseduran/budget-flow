using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Commands.UpdateAssetType
{
    public class UpdateAssetTypeCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public AssetTypeDto AssetType { get; set; }
        public class UpdateAssetTypeCommandHandler : IRequestHandler<UpdateAssetTypeCommand, bool>
        {
            private readonly IAssetTypeRepository assetTypeRepository;
            public UpdateAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository)
            {
                this.assetTypeRepository = assetTypeRepository;
            }

            public Task<bool> Handle(UpdateAssetTypeCommand request, CancellationToken cancellationToken)
            {
                var result = assetTypeRepository.UpdateAssetTypeAsync(request.ID, request.AssetType);
                return result;
            }
        }
    }
}
