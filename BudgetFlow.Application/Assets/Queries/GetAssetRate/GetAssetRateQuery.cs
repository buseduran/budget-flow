using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Assets.Queries.GetAssetRate;
public class GetAssetRateQuery : IRequest<Result<AssetRateResponse>>
{
    public int ID { get; set; }
    public GetAssetRateQuery(int ID)
    {
        this.ID = ID;
    }
    public class GetAssetRateQueryHandler : IRequestHandler<GetAssetRateQuery, Result<AssetRateResponse>>
    {
        private readonly IAssetRepository assetRepository;
        public GetAssetRateQueryHandler(IAssetRepository assetRepository)
        {
            this.assetRepository = assetRepository;
        }

        public async Task<Result<AssetRateResponse>> Handle(GetAssetRateQuery request, CancellationToken cancellationToken)
        {
            var rate = await assetRepository.GetAssetRateAsync(request.ID);
            if (rate == null)
                return Result.Failure<AssetRateResponse>(AssetErrors.AssetRateNotFound);

            var response = new AssetRateResponse
            {
                BuyPrice = rate.BuyPrice,
                SellPrice = rate.SellPrice
            };
            return Result.Success(response);
        }
    }
}
