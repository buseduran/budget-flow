using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
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
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrentUserService _currentUserService;
        public DeletePortfolioCommandHandler(IPortfolioRepository portfolioRepository, ICurrentUserService currentUserService)
        {
            _portfolioRepository = portfolioRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeletePortfolioCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var result = await _portfolioRepository.DeletePortfolioAsync(request.ID, userID);
            return result
            ? Result.Success(result)
            : Result.Failure<bool>(PortfolioErrors.PortfolioDeletionFailed);
        }
    }
}
