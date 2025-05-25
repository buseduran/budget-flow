using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Enums;
using Quartz;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Application.Common.Results;

namespace BudgetFlow.Application.Common.Jobs;
public class MetalJob : IJob
{
    private readonly IMetalScraper metalScraper;
    private readonly IAssetRepository assetRepository;
    private readonly IUnitOfWork unitOfWork;

    public MetalJob(
        IMetalScraper metalScraper,
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        this.metalScraper = metalScraper;
        this.assetRepository = assetRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        var assetType = AssetType.Metal;
        var metals = await metalScraper.GetMetalsAsync(assetType);

        await unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var metal in metals)
            {
                var existingAsset = await assetRepository.GetByCodeAsync(metal.Code);
                if (existingAsset != null)
                {
                    // Update existing asset
                    var asset = new Asset
                    {
                        ID = existingAsset.ID,
                        Name = existingAsset.Name,
                        AssetType = existingAsset.AssetType,
                        BuyPrice = metal.BuyPrice,
                        SellPrice = metal.SellPrice,
                        Description = existingAsset.Description,
                        Symbol = existingAsset.Symbol,
                        Code = existingAsset.Code,
                        Unit = existingAsset.Unit,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await assetRepository.UpdateAssetAsync(asset);
                }
                else
                {
                    // Create new asset
                    metal.CreatedAt = DateTime.UtcNow;
                    metal.UpdatedAt = DateTime.UtcNow;
                    await assetRepository.CreateAssetAsync(metal);
                }
            }

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
    }
}
