﻿using BudgetFlow.Application.Common.Behaviors;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Jobs;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Services.Concrete;
using BudgetFlow.Application.Common.Utils;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
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

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddHttpContextAccessor();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

        services.AddScoped<IStockScraper, StockScraper>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IWalletAuthService, WalletAuthService>();

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("StockJob");

            q.AddJob<StockJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("StockJob-trigger")
                .WithCronSchedule("0 0/30 * * * ?")); // Every 30 minutes  
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

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
