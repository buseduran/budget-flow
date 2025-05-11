using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Jobs;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Services.Concrete;
using BudgetFlow.Application.Common.Utils;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Reflection;

namespace BudgetFlow.Application;

public static class ServiceRegistration
{
    public static void AddApplication(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddHttpContextAccessor();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

        services.AddScoped<IStockScraper, StockScraper>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("StockJob");

            q.AddJob<StockJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("StockJob-trigger")
                .WithCronSchedule("0 0/30 * * * ?")); // Every 30 minutes  
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
