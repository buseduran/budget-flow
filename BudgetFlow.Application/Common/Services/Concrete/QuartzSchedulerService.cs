using BudgetFlow.Application.Common.Jobs;
using Quartz;
using Quartz.Impl;

namespace BudgetFlow.Application.Common.Services.Concrete;
public class QuartzSchedulerService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IScheduler _scheduler;
    public QuartzSchedulerService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
        _scheduler = schedulerFactory.GetScheduler().Result;
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

        #region Metal Job
        IJobDetail metalJob = JobBuilder.Create<MetalJob>()
            .WithIdentity("metalJob", "group1")
            .Build();

        ITrigger metalTrigger = TriggerBuilder.Create()
            .WithIdentity("metalTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(60) // Her dakika başı
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(metalJob, metalTrigger);
        #endregion
    }
    public async Task StopAsync()
    {
        await _scheduler.Shutdown();
    }
    public async Task ScheduleJob(IJobDetail jobDetail, ITrigger trigger)
    {
        await _scheduler.ScheduleJob(jobDetail, trigger);
    }
}
