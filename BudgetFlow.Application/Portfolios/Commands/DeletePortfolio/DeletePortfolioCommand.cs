using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Commands.DeletePortfolio;
public class DeletePortfolioCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public DeletePortfolioCommand(int ID)
    {
        this.ID = ID;
    }
    public class DeletePortfolioCommandHandler : IRequestHandler<DeletePortfolioCommand, Result<bool>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        public DeletePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<Result<bool>> Handle(DeletePortfolioCommand request, CancellationToken cancellationToken)
        {
            var result = await portfolioRepository.DeletePortfolioAsync(request.ID);
            return result
            ? Result.Success(result)
            : Result.Failure<bool>("Error deleting portfolio");
        }
    }
}
