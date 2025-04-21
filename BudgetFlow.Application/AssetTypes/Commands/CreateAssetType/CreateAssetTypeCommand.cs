using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Commands.CreateAssetType
{
    public class CreateAssetTypeCommand : IRequest<Result<bool>>
    {
        public AssetTypeDto AssetTypeDto { get; set; }
        public class CreateAssetTypeCommandHandler : IRequestHandler<CreateAssetTypeCommand, Result<bool>>
        {
            private readonly IAssetTypeRepository assetRepository;
            public CreateAssetTypeCommandHandler(IAssetTypeRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }

            public async Task<Result<bool>> Handle(CreateAssetTypeCommand request, CancellationToken cancellationToken)
            {
                if (String.IsNullOrEmpty(request.AssetTypeDto.Name))
                    return Result.Failure<bool>("Asset name cannot be empty");


                AssetType assetType = new()
                {
                    Name = request.AssetTypeDto.Name,
                    Description = request.AssetTypeDto.Description,
                };
                var result = await assetRepository.CreateAssetTypeAsync(assetType);
                return result
                    ? Result.Success(result)
                    : Result.Failure<bool>("Error creating asset type");
            }
        }
    }
}
