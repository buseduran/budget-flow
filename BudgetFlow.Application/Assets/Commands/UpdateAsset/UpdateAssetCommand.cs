using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Assets.Commands.UpdateAsset;
public class UpdateAssetCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public IFormFile Symbol { get; set; }
    public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Result<bool>>
    {
        private readonly IAssetRepository _assetRepository;
        public UpdateAssetCommandHandler(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }
        public async Task<Result<bool>> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
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
                ID = request.ID,
                Symbol = image,
            };

            var result = await _assetRepository.UpdateAssetAsync(asset);

            return result
                ? Result.Success(true)
                : Result.Failure<bool>(AssetErrors.AssetUpdateFailed);
        }
    }
}
