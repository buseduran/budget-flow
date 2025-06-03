using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Wallets.Queries.GetWalletAssets;
public class GetWalletAssetsQuery : IRequest<Result<List<GetWalletAssetsResponse>>>
{
    public int WalletID { get; set; }
    public class GetWalletAssetsQueryHandler : IRequestHandler<GetWalletAssetsQuery, Result<List<GetWalletAssetsResponse>>>
    {
        private readonly IWalletRepository _walletRepository;
        public GetWalletAssetsQueryHandler(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }
        public async Task<Result<List<GetWalletAssetsResponse>>> Handle(GetWalletAssetsQuery request, CancellationToken cancellationToken)
        {
            var walletAssets = await _walletRepository.GetWalletAssetsAsync(request.WalletID);
            var assets = walletAssets.Select(wa => new GetWalletAssetsResponse { ID = wa.AssetId, Name = wa.Asset.Name }).ToList();
            return Result.Success(assets);
        }
    }
}