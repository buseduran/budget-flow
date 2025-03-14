using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly BudgetContext context;
        private readonly IMapper mapper;
        public InvestmentRepository(BudgetContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<bool> CreateInvestmentAsync(Investment Investment)
        {
            Investment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            Investment.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            await context.Investments.AddAsync(Investment);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteInvestmentAsync(int ID)
        {
            return await context.Investments
                  .Where(e => e.ID == ID)
                  .ExecuteDeleteAsync() > 0;
        }

        public async Task<bool> UpdateInvestmentAsync(int ID, InvestmentDto Investment)
        {
            var investment = await context.Investments.FindAsync(ID);
            if (investment is null) return false;

            mapper.Map(Investment, investment);
            investment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<List<InvestmentResponse>> GetInvestmentsAsync(int PortfolioID)
        {
            var investments = await context.Investments
                .Where(e => e.PortfolioId == PortfolioID)
                .Include(e => e.Asset)
                .Select(i => new InvestmentResponse
                {
                    ID = i.ID,
                    Name = i.Asset.Name,
                    Amount = i.Amount,
                    Description = i.Description,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                }).ToListAsync();
            return investments;
        }

        public async Task<List<LastInvestmentResponse>> GetLastInvestmentsAsync(string Portfolio)
        {
            var investments = await context.Investments
               .Include(e => e.Portfolio)
               .Where(e => e.Portfolio.Name == Portfolio)
               .Include(e => e.Asset)
               .OrderByDescending(e => e.CreatedAt)
               .Take(5)
               .Select(e => new LastInvestmentResponse
               {
                   ID = e.ID,
                   Name = e.Asset.Name,
                   Amount = e.Amount,
                   Description = e.Description,
                   CreatedAt = e.CreatedAt,
                   UpdatedAt = e.UpdatedAt
               })
               .ToListAsync();
            return investments;
        }
    }
}
