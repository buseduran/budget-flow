using BudgetFlow.Application.Categories;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<bool> CreateCategoryAsync(Category categoryDto);
        Task<IEnumerable<CategoryResponse>> GetCategoriesAsync();
        Task<bool> UpdateCategoryAsync(int ID, string Color);
        Task<bool> DeleteCategoryAsync(int ID);
    }
}
