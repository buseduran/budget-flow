using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Assets.Commands.CreateAsset;
public class CreateAssetCommand : IRequest<Result<bool>>
{
    public AssetDto Asset { get; set; }
    public IFormFile Symbol { get; set; }
    public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, Result<bool>>
    {
        private readonly IAssetRepository assetRepository;
        public CreateAssetCommandHandler(IAssetRepository assetRepository)
        {
            this.assetRepository = assetRepository;
        }
        public async Task<Result<bool>> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
        {
            string image = string.Empty;
            if (request.Symbol != null && request.Symbol.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await request.Symbol.CopyToAsync(memoryStream, cancellationToken);
                image = Convert.ToBase64String(memoryStream.ToArray());
            }

            Asset asset = new()
            {
                Name = request.Asset.Name,
                AssetType = request.Asset.AssetType,
                BuyPrice = request.Asset.BuyPrice,
                SellPrice = request.Asset.SellPrice,
                Description = request.Asset.Description,
                Symbol = image,
                Code = request.Asset.Code,
                Unit = request.Asset.Unit
            };
            var result = await assetRepository.CreateAssetAsync(asset);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(AssetErrors.CreationFailed);
        }
    }
}
