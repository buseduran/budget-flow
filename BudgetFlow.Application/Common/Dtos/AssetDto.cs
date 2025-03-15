using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Common.Dtos
{
    public class AssetDto
    {
        public string Name { get; set; }
        public int AssetTypeId { get; set; }
        public decimal CurrentPrice { get; set; }
        public string? Description { get; set; }
        public IFormFile Symbol { get; set; }
    }
}
