using AutoMapper;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly BudgetContext context;

        public PortfolioRepository(BudgetContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreatePortfolioAsync(Portfolio Portfolio)
        {
            Portfolio.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Portfolio.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            await context.Portfolios.AddAsync(Portfolio);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
