using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace BudgetFlow.Infrastructure.Common.Persistence;
public class UnitOfWork : IUnitOfWork
{
    private readonly BudgetContext context;
    private IDbContextTransaction transaction;
    public UnitOfWork(BudgetContext context)
    {
        this.context = context;
    }

    public async Task BeginTransactionAsync()
    {
        if (transaction == null)
            transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (transaction != null)
        {
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (transaction != null)
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();
}
