using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Application.Statistics.Responses;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class InvestmentRepository : IInvestmentRepository
{
    private readonly BudgetContext context;
    public InvestmentRepository(BudgetContext context)
    {
        this.context = context;
    }

    public async Task<bool> CreateInvestmentAsync(Investment Investment, bool saveChanges = true)
    {
        Investment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        Investment.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        await context.Investments.AddAsync(Investment);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteInvestmentAsync(int ID)
    {
        var investment = await context.Investments.FindAsync(ID);
        if (investment is null) return false;

        context.Investments.Remove(investment);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateInvestmentAsync(int ID, InvestmentDto Investment)
    {
        var investment = await context.Investments.FindAsync(ID);
        if (investment is null) return false;

        investment.UnitAmount = Investment.UnitAmount;
        investment.CurrencyAmount = Investment.CurrencyAmount;
        investment.Description = Investment.Description;
        investment.Date = Investment.Date;

        investment.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<PaginatedList<InvestmentPaginationResponse>> GetInvestmentsAsync(int Page, int PageSize, int PortfolioID, int? AssetId = null, int? UserId = null)
    {
        var query = context.Investments
            .Where(e => e.PortfolioId == PortfolioID);

        if (AssetId.HasValue)
        {
            query = query.Where(e => e.AssetId == AssetId.Value);
        }

        if (UserId.HasValue)
        {
            query = query.Where(e => e.UserId == UserId.Value);
        }

        var investments = await query
            .OrderByDescending(e => e.Date)
            .Include(e => e.Asset)
            .Include(e => e.User)
            .Select(i => new InvestmentPaginationResponse
            {
                ID = i.ID,
                Name = i.Asset.Name,
                UnitAmount = i.UnitAmount,
                Unit = i.Asset.Unit,
                CurrencyAmount = i.CurrencyAmount,
                Type = i.Type,
                ExchangeRate = i.ExchangeRate,
                Description = i.Description,
                Date = i.Date,
                UserName = i.User.Name,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
            }).ToListAsync();
        var count = investments.Count();
        return new PaginatedList<InvestmentPaginationResponse>(investments, count, Page, PageSize);
    }

    public async Task<InvestmentResponse> GetInvestmentByIdAsync(int ID)
    {
        var investment = await context.Investments
            .Where(e => e.ID == ID)
            .Include(e => e.Asset)
            .Select(i => new InvestmentResponse
            {
                ID = i.ID,
                Name = i.Asset.Name,
                CurrencyAmount = i.CurrencyAmount,
                UnitAmount = i.UnitAmount,
                Description = i.Description,
                Date = i.Date,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                AssetID = i.AssetId,
                PortfolioID = i.PortfolioId,
                Type = i.Type,
            }).FirstOrDefaultAsync();
        return investment;
    }
}
