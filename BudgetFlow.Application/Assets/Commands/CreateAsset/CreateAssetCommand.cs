using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Assets.Commands.CreateAsset
{
    public class CreateAssetCommand : IRequest<bool>
    {
        public AssetDto Asset { get; set; }
        public IFormFile Symbol { get; set; }
        public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, bool>
        {
            private readonly IAssetRepository assetRepository;
            public CreateAssetCommandHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public async Task<bool> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
            {
                if (request.Asset.Symbol != null && request.Asset.Symbol.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await request.Symbol.CopyToAsync(memoryStream, cancellationToken);
                    request.Asset.Symbol = Convert.ToBase64String(memoryStream.ToArray());
                }

                Asset asset = new()
                {
                    Name = request.Asset.Name,
                    AssetTypeId = request.Asset.AssetTypeId,
                    BuyPrice = request.Asset.BuyPrice,
                    SellPrice = request.Asset.SellPrice,
                    Description = request.Asset.Description,
                    Symbol = request.Asset.Symbol
                };
                var result = await assetRepository.CreateAssetAsync(asset);
                if (!result)
                    throw new Exception("Failed to create asset.");
                return true;
            }
        }
    }
}
