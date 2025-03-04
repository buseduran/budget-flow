using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetFlow.Infrastructure;
public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
    }
}
