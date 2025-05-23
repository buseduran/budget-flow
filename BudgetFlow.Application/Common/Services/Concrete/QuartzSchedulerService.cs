﻿using BudgetFlow.Application.Common.Jobs;
using Quartz;
using Quartz.Impl;

namespace BudgetFlow.Application.Common.Services.Concrete;
public class QuartzSchedulerService
{
    private readonly ISchedulerFactory schedulerFactory;
    private readonly IScheduler scheduler;
    public QuartzSchedulerService(ISchedulerFactory schedulerFactory)
    {
        this.schedulerFactory = schedulerFactory;
        this.scheduler = schedulerFactory.GetScheduler().Result;
    }
    public static async Task StartAsync(IServiceProvider serviceProvider)
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        #region Stock Job
        IJobDetail jobDetail = JobBuilder.Create()
          .WithIdentity("stockJob", "group1")
          .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("stockTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(1800) //30 minutes
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger);
        #endregion

        #region Currency Job
        IJobDetail currencyJob = JobBuilder.Create<CurrencyJob>()
       .WithIdentity("currencyJob", "group1")
       .Build();

        ITrigger currencyTrigger = TriggerBuilder.Create()
            .WithIdentity("currencyTrigger", "group1")
            .StartNow()
            .WithDailyTimeIntervalSchedule(x =>
                x.WithIntervalInHours(24)
                 .OnEveryDay()
                 .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(16, 0))) // her gün 16:00
            .Build();

        await scheduler.ScheduleJob(currencyJob, currencyTrigger);
        #endregion
    }
    public async Task StopAsync()
    {
        await scheduler.Shutdown();
    }
    public async Task ScheduleJob(IJobDetail jobDetail, ITrigger trigger)
    {
        await scheduler.ScheduleJob(jobDetail, trigger);
    }
}
