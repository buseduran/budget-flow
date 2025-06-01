using BudgetFlow.Application.Common.Behaviors;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Jobs;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Services.Concrete;
using BudgetFlow.Application.Common.Utils;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Quartz;
using System.Reflection;
using BudgetFlow.Application.Common.Scrapers.Abstract;
using BudgetFlow.Application.Common.Scrapers.Concrete;

namespace BudgetFlow.Application;

public static class ServiceRegistration
{
    public static void AddApplication(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

        services.AddScoped<IStockScraper, StockScraper>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IWalletAuthService, WalletAuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IGeminiService, GeminiService>();

        services.AddHttpClient<IExchangeRateScraper, ExchangeRateScraper>();
        services.AddHttpClient<IMetalScraper, MetalScraper>();
        services.AddHttpClient<IStockScraper, StockScraper>();

        services.AddScoped<CurrencyJob>();
        services.AddScoped<MetalJob>();
        services.AddScoped<StockJob>();
        services.AddScoped<GeminiAnalysisJob>();

        #region Jobs
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("StockJob");
            q.AddJob<StockJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("StockJob-trigger")
                .WithCronSchedule("0 */15 * * * ?")); // Every 15 minutes
        });

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("CurrencyJob");
            q.AddJob<CurrencyJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("CurrencyJob-trigger")
                .WithCronSchedule("0 0 16 * * ?")); // Every day at 16:00
        });

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("MetalJob");
            q.AddJob<MetalJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("MetalJob-trigger")
                .WithCronSchedule("0 */15 * * * ?")); // Every 15 minutes
        });

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("GeminiAnalysisJob");
            q.AddJob<GeminiAnalysisJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("GeminiAnalysisJob-trigger")
                .WithCronSchedule("0 0 4 * * ?")); // Every day at 04:00
        });


        #endregion

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
