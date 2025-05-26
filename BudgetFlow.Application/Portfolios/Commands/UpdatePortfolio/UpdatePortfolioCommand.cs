using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Commands.UpdatePortfolio;
public class UpdatePortfolioCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public class UpdatePortfolioCommandHandler : IRequestHandler<UpdatePortfolioCommand, Result<bool>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdatePortfolioCommandHandler(IPortfolioRepository portfolioRepository, ICurrentUserService currentUserService)
        {
            _portfolioRepository = portfolioRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(UpdatePortfolioCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var result = await _portfolioRepository.UpdatePortfolioAsync(request.ID, request.Name, request.Description, userID);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(PortfolioErrors.PortfolioUpdateFailed);
        }
    }
}
