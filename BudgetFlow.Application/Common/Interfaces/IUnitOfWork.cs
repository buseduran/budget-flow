using Microsoft.EntityFrameworkCore;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces;
public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
    DbContext GetDbContext();
    DbSet<Asset> GetAssets();
}
