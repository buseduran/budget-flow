using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Common.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace BudgetFlow.Infrastructure.Contexts;

public class BudgetContext : DbContext
{
    private readonly IConfiguration configuration;
    private readonly AuditInterceptor auditInterceptor;

    public BudgetContext(DbContextOptions<BudgetContext> options, IConfiguration configuration, AuditInterceptor auditInterceptor)
        : base(options)
    {
        this.configuration = configuration;
        this.auditInterceptor = auditInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = configuration.GetConnectionString("DbConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }

        optionsBuilder.AddInterceptors(auditInterceptor);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Investment> Investments { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletAsset> WalletAssets { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<CurrencyRate> CurrencyRates { get; set; }
    public DbSet<SummaryReport> SummaryReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Portfolio>()
            .HasIndex(x => x.Name)
            .IsUnique();

        // Configure cascade delete for User
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserWallets)
            .WithOne(uw => uw.User)
            .HasForeignKey(uw => uw.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Portfolios)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Categories)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Entries)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure cascade delete for Category
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Entries)
            .WithOne(e => e.Category)
            .HasForeignKey(e => e.CategoryID)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
