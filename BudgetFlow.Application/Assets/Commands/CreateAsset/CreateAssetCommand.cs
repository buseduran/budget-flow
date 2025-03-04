using AutoMapper;
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
            private readonly IMapper mapper;
            public CreateAssetCommandHandler(IAssetRepository assetRepository, IMapper mapper)
            {
                this.assetRepository = assetRepository;
                this.mapper = mapper;
            }
            public async Task<bool> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
            {
                var mappedAsset = mapper.Map<Asset>(request.Asset);
                var result = await assetRepository.CreateAssetAsync(mappedAsset);
                if (!result)
                    throw new Exception("Failed to create asset.");
                return true;
            }
        }
    }
}
