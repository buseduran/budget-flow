using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Commands.CreateAssetType
{
    public class CreateAssetTypeCommand : IRequest<bool>
    {
        public AssetTypeDto AssetTypeDto { get; set; }
        public class CreateAssetTypeCommandHandler : IRequestHandler<CreateAssetTypeCommand, bool>
        {
            private readonly IAssetTypeRepository assetRepository;
            public Task<bool> Handle(CreateAssetTypeCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
