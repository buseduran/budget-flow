using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Services.Concrete;
using BudgetFlow.Application.Common.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BudgetFlow.Application;
public static class ServiceRegistration
{
    public static void AddApplication(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddHttpContextAccessor();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        services.AddSingleton<IStockScraper, StockScraper>();
    }
}

