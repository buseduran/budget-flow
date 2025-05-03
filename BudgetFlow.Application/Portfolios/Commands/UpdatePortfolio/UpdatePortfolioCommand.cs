using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Commands.UpdatePortfolio;
public class UpdatePortfolioCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public PortfolioDto Portfolio { get; set; }
    public class UpdatePortfolioCommandHandler : IRequestHandler<UpdatePortfolioCommand, Result<bool>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        public UpdatePortfolioCommandHandler(IPortfolioRepository portfolioRepository)
        {
            this.portfolioRepository = portfolioRepository;
        }

        public async Task<Result<bool>> Handle(UpdatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var result = await portfolioRepository.UpdatePortfolioAsync(request.ID, request.Portfolio);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(PortfolioErrors.PortfolioUpdateFailed);
        }
    }
}
