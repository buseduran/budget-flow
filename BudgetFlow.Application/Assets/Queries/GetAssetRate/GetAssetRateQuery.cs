using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BudgetFlow.Application.Assets.Queries.GetAssetRate
{
    public class GetAssetRateQuery : IRequest<IActionResult>
    {
        public int ID { get; set; }
        public GetAssetRateQuery(int ID)
        {
            this.ID = ID;
        }
        public class GetAssetRateQueryHandler : IRequestHandler<GetAssetRateQuery, IActionResult>
        {
            private readonly IAssetRepository assetRepository;
            public GetAssetRateQueryHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }

            public async Task<IActionResult> Handle(GetAssetRateQuery request, CancellationToken cancellationToken)
            {
                var result = await assetRepository.GetAssetRateAsync(request.ID);
                var jsonResult = new { BuyPrice = result.BuyPrice, SellPrice = result.SellPrice };

                return new JsonResult(jsonResult);
            }
        }
    }
}
