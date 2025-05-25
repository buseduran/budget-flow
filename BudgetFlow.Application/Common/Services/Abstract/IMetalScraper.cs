using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Services.Abstract;
public interface IMetalScraper
{
    Task<IEnumerable<Asset>> GetMetalsAsync(AssetType assetType);
}
