using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Commands.DeletePortfolio
{
    public class DeletePortfolioCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public DeletePortfolioCommand(int ID)
        {
            this.ID = ID;
        }
        public class DeletePortfolioCommandHandler : IRequestHandler<DeletePortfolioCommand, bool>
        {
            private readonly IPortfolioRepository portfolioRepository;
            public DeletePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
            {
                this.portfolioRepository = portfolioRepository;
            }

            public async Task<bool> Handle(DeletePortfolioCommand request, CancellationToken cancellationToken)
            {
                var result = await portfolioRepository.DeletePortfolioAsync(request.ID);
                return result;
            }
        }
    }
}
