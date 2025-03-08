using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Portfolios.Commands.CreatePortfolio
{
    public class CreatePortfolioCommand : IRequest<bool>
    {
        public PortfolioDto Portfolio { get; set; }
        public class CreatePortfolioCommandHandler : IRequestHandler<CreatePortfolioCommand, bool>
        {
            private readonly IPortfolioRepository portfolioRepository;
            private readonly IMapper mapper;
            private readonly IHttpContextAccessor httpContextAccessor;
            public CreatePortfolioCommandHandler(IPortfolioRepository portfolioRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            {
                this.portfolioRepository = portfolioRepository;
                this.mapper = mapper;
                this.httpContextAccessor = httpContextAccessor;
            }
            public async Task<bool> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
            {
                var portfolio = mapper.Map<Portfolio>(request.Portfolio);
                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                portfolio.UserID = getCurrentUser.GetCurrentUserID();

                var result = await portfolioRepository.CreatePortfolioAsync(portfolio);
                if (!result)
                {
                    throw new Exception("Failed to create Portfolio");
                }
                return result;
            }
        }
    }
}
