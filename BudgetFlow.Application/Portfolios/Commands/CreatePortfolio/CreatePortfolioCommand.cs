using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;
public class CreatePortfolioCommand : IRequest<Result<int>>
{
    public PortfolioDto Portfolio { get; set; }
    public class CreatePortfolioCommandHandler : IRequestHandler<CreatePortfolioCommand, Result<int>>
    {
        private readonly IPortfolioRepository portfolioRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public CreatePortfolioCommandHandler(IPortfolioRepository portfolioRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.portfolioRepository = portfolioRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<int>> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            Portfolio portfolio = new()
            {
                Name = request.Portfolio.Name,
                Description = request.Portfolio.Description
            };
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            portfolio.UserID = getCurrentUser.GetCurrentUserID();

            var result = await portfolioRepository.CreatePortfolioAsync(portfolio);
            return result == 0
                ? Result.Failure<int>("Failed to create Portfolio")
                : Result.Success(result);
        }
    }
}
