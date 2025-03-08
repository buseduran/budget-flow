using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Portfolios;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly BudgetContext context;
        private readonly IMapper mapper;

        public PortfolioRepository(BudgetContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> CreatePortfolioAsync(Portfolio Portfolio)
        {
            Portfolio.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Portfolio.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            await context.Portfolios.AddAsync(Portfolio);
            return await context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeletePortfolioAsync(int ID)
        {
            return await context.Portfolios
                    .Where(e => e.ID == ID)
                    .ExecuteDeleteAsync() > 0;
        }

        public async Task<bool> UpdatePortfolioAsync(int ID, PortfolioDto Portfolio)
        {
            var portfolio = await context.Portfolios.FindAsync(ID);
            if (portfolio is null) return false;

            mapper.Map(Portfolio, portfolio);
            portfolio.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            return await context.SaveChangesAsync() > 0;
        }
        public async Task<List<PortfolioResponse>> GetPortfoliosAsync(int UserID)
        {
            //calculate investments analysis here
            var portfolios = await context.Portfolios
                .Where(e => e.UserID == UserID)
                .Include(e => e.Investments)
                .Select(e => new PortfolioResponse
                {
                    ID = e.ID,
                    Name = e.Name,
                    Description = e.Description,
                    TotalInvested = e.Investments.Sum(i => i.Amount)
                })
                .ToListAsync();
            return portfolios;
        }
    }
}
