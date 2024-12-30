using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BudgetFlow.Infrastructure.Contexts
{
    public class BudgetContext(IConfiguration configuration) : DbContext
    {
        private readonly string connectionString = configuration.GetConnectionString("DbConnection");
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(connectionString);

        public DbSet<UserDto> Users { get; set; }
        public DbSet<BudgetDto> Budgets { get; set; }
        public DbSet<LogDto> Logs { get; set; }
        public DbSet<TransactionDto> Transactions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
