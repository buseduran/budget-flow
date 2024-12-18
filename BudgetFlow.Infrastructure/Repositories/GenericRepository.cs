using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class, new()
    {
        protected readonly BudgetContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(BudgetContext context, DbSet<TEntity> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }
    }
}
