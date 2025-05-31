using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Infrastructure.Common.Persistence;
using BudgetFlow.Infrastructure.Common.Persistence.Interceptors;
using BudgetFlow.Infrastructure.Repositories;
using BudgetFlow.Infrastructure.Seed;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetFlow.Infrastructure;
public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        #region Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IInvestmentRepository, InvestmentRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IUserWalletRepository, UserWalletRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();
        services.AddScoped<IWalletAssetRepository, WalletAssetRepository>();
        #endregion

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<AdminUserSeeder>();

        //AuditInterceptor kaydı
        services.AddScoped<AuditInterceptor>();
    }
}
