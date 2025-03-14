using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Commands.CreateAssetType
{
    public class CreateAssetTypeCommand : IRequest<bool>
    {
        public AssetTypeDto AssetTypeDto { get; set; }
        public class CreateAssetTypeCommandHandler : IRequestHandler<CreateAssetTypeCommand, bool>
        {
            private readonly IAssetTypeRepository assetRepository;
            public CreateAssetTypeCommandHandler(IAssetTypeRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }

            public async Task<bool> Handle(CreateAssetTypeCommand request, CancellationToken cancellationToken)
            {
                AssetType assetType = new()
                {
                    Name = request.AssetTypeDto.Name,
                    Description = request.AssetTypeDto.Description,
                };
                var result = await assetRepository.CreateAssetTypeAsync(assetType);
                return result;
            }
        }
    }
}
