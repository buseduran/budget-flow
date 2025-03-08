using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.UpdateAsset
{
    public class UpdateAssetCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public AssetDto Asset { get; set; }
        public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, bool>
        {
            private readonly IAssetRepository assetRepository;
            public UpdateAssetCommandHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public Task<bool> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
            {
                var result = assetRepository.UpdateAssetAsync(request.ID, request.Asset);
                return result;
            }
        }
    }
}
