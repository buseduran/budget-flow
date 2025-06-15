using BudgetFlow.Application.Categories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface ICategoryRepository
{
    Task<int> CreateCategoryAsync(Category category, bool saveChanges = true);
    Task<PaginatedList<CategoryResponse>> GetCategoriesAsync(int page, int pageSize, int walletID);
    Task<CategoryResponse> GetCategoryByIDAsync(int ID);
    Task<bool> UpdateCategoryAsync(int ID, string color, int walletID);
    Task<bool> DeleteCategoryAsync(int ID);
}
