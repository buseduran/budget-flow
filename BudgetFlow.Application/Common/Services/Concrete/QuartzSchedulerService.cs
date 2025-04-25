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
    public async Task StartAsync(IServiceProvider serviceProvider)
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        IJobDetail jobDetail = JobBuilder.Create()
            .WithIdentity("stockJob", "group1")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("stockTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(10) // 1800 seconds = 30 minutes
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger);
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
