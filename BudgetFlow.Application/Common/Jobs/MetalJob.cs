using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Enums;
using Quartz;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Jobs;
public class MetalJob : IJob
{
    private readonly IMetalScraper _metalScraper;
    private readonly IAssetRepository _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MetalJob(
        IMetalScraper metalScraper,
        IAssetRepository assetRepository,
        IUnitOfWork unitOfWork)
    {
        _metalScraper = metalScraper;
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await ExecuteAsync();
    }

    public async Task ExecuteAsync()
    {
        var assetType = AssetType.Metal;
        var metals = await _metalScraper.GetMetalsAsync(assetType);
        Console.WriteLine(metals);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var metal in metals)
            {
                var existingAsset = await _assetRepository.GetByCodeAsync(metal.Code);
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
                    await _assetRepository.UpdateAssetAsync(asset);
                }
                else
                {
                    // Create new asset
                    metal.CreatedAt = DateTime.UtcNow;
                    metal.UpdatedAt = DateTime.UtcNow;
                    await _assetRepository.CreateAssetAsync(metal);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
