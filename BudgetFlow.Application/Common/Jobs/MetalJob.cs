using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using Quartz;

namespace BudgetFlow.Application.Common.Jobs;
public class MetalJob : IJob
{
    private readonly IMetalScraper metalScraper;
    public MetalJob(IMetalScraper metalScraper)
    {
        this.metalScraper = metalScraper;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var assetType = AssetType.Metal;
        var values= await metalScraper.GetMetalsAsync(assetType);

        // You can process the retrieved metal values here, such as saving them to a database or logging them.
    }
}
