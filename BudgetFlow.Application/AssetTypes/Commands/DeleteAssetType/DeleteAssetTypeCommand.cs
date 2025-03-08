using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.AssetTypes.Commands.DeleteAssetType
{
    public class DeleteAssetTypeCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public DeleteAssetTypeCommand(int ID)
        {
            this.ID = ID;
        }

        public class DeleteAssetTypeCommandHandler : IRequestHandler<DeleteAssetTypeCommand, bool>
        {
            private readonly IAssetTypeRepository assetTypeRepository;
            public DeleteAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository)
            {
                this.assetTypeRepository = assetTypeRepository;
            }
            public Task<bool> Handle(DeleteAssetTypeCommand request, CancellationToken cancellationToken)
            {
                var result = assetTypeRepository.DeleteAssetTypeAsync(request.ID);
                return result;
            }
        }

    }
}
