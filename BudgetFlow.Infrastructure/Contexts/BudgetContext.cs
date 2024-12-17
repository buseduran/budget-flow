using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BudgetFlow.Infrastructure.Contexts
{
    public class BudgetContext(IConfiguration configuration) : DbContext
    {
        private readonly string connectionString = configuration.GetConnectionString("DbConnection");
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(connectionString);

        public DbSet<User> Users { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
