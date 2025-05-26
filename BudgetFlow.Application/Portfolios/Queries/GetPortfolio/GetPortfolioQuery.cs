using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Portfolios.Queries.GetPortfolio;
public class GetPortfolioQuery : IRequest<Result<PortfolioResponse>>
{
    public string Name { get; set; }
    public GetPortfolioQuery(string Name)
    {
        this.Name = Name;
    }
    public class GetPortfolioQueryHandler : IRequestHandler<GetPortfolioQuery, Result<PortfolioResponse>>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetPortfolioQueryHandler(IPortfolioRepository portfolioRepository, ICurrentUserService currentUserService)
        {
            _portfolioRepository = portfolioRepository;
            _currentUserService = currentUserService;
        }
        public async Task<Result<PortfolioResponse>> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var portfolio = await _portfolioRepository.GetPortfolioAsync(request.Name, userID);
            return portfolio != null
                ? Result.Success(portfolio)
                : Result.Failure<PortfolioResponse>(PortfolioErrors.PortfolioNotFound);
        }
    }
}
