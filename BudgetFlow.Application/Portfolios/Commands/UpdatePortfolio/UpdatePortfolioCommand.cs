using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Commands.UpdatePortfolio
{
    public class UpdatePortfolioCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public PortfolioDto Portfolio { get; set; }
        public class UpdatePortfolioCommandHandler : IRequestHandler<UpdatePortfolioCommand, bool>
        {
            private readonly IPortfolioRepository portfolioRepository;
            public UpdatePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
            {
                this.portfolioRepository = portfolioRepository;
            }

            public async Task<bool> Handle(UpdatePortfolioCommand request, CancellationToken cancellationToken)
            {
                var result = await portfolioRepository.UpdatePortfolioAsync(request.ID, request.Portfolio);
                return result;
            }
        }
    }
}
