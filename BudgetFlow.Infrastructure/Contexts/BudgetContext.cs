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

        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<UserAsset> UserAssets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(User => User.Email)
                .IsUnique();
            modelBuilder.Entity<Portfolio>()
                .HasIndex(Portfolio => Portfolio.Name)
                .IsUnique();
        }
    }
}
