using BudgetFlow.Application.Category;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<bool> CreateCategoryAsync(Domain.Entities.CategoryEntity categoryDto);
        Task<IEnumerable<CategoryResponse>> GetCategoriesAsync();
        Task<bool> UpdateCategoryAsync(int ID, string Color);
        Task<bool> DeleteCategoryAsync(int ID);
    }
}
