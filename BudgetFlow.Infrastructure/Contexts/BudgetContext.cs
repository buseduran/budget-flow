using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BudgetFlow.Infrastructure.Persistence.Interceptors;
using System.Reflection;
using BudgetFlow.Application.Common.Models;

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
    public DbSet<UserAsset> UserAssets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Portfolio>()
            .HasIndex(x => x.Name)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
