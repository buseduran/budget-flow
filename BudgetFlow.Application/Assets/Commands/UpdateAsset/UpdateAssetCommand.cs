using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Assets.Commands.UpdateAsset
{
    public class UpdateAssetCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public AssetDto Asset { get; set; }
        public IFormFile Symbol { get; set; }
        public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, bool>
        {
            private readonly IAssetRepository assetRepository;
            public UpdateAssetCommandHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public async Task<bool> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
            {
                if (request.Asset.Symbol != null && request.Asset.Symbol.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await request.Symbol.CopyToAsync(memoryStream, cancellationToken);
                    request.Asset.Symbol = Convert.ToBase64String(memoryStream.ToArray());
                }
                var result = await assetRepository.UpdateAssetAsync(request.ID, request.Asset);

                return result;
            }
        }
    }
}
