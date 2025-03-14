using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.CreateAsset
{
    public class CreateAssetCommand : IRequest<bool>
    {
        public AssetDto Asset { get; set; }
        public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, bool>
        {
            private readonly IAssetRepository assetRepository;
            public CreateAssetCommandHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public async Task<bool> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
            {
                Asset asset = new()
                {
                    Name = request.Asset.Name,
                    AssetTypeId = request.Asset.AssetTypeId,
                    CurrentPrice = request.Asset.CurrentPrice,
                    Description = request.Asset.Description
                };
                var result = await assetRepository.CreateAssetAsync(asset);
                if (!result)
                    throw new Exception("Failed to create asset.");
                return true;
            }
        }
    }
}
