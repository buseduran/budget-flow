using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Assets.Commands.DeleteAsset
{
    public class DeleteAssetCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public DeleteAssetCommand(int ID)
        {
            this.ID = ID;
        }
        public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, bool>
        {
            private readonly IAssetRepository assetRepository;
            public DeleteAssetCommandHandler(IAssetRepository assetRepository)
            {
                this.assetRepository = assetRepository;
            }
            public Task<bool> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
            {
                var result = assetRepository.DeleteAssetAsync(request.ID);
                return result;
            }
        }
    }
}
