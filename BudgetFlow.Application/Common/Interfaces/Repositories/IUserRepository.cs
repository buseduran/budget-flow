using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Users;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IUserRepository
{
    Task<int> CreateAsync(User user, bool saveChanges = true);
    Task<UserResponse> GetByIdAsync(int ID);
    Task<UserResponse> GetByEmailAsync(string email);
    Task<bool> UpdateAsync(string Name, string OldEmail, string Email, string PasswordHash);
    Task<bool> UpdateWithoutPasswordAsync(string Name, string OldEmail, string Email);
    Task<bool> DeleteAsync(int ID);
    Task<bool> CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string token);
    Task<bool> UpdateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenByUserIDAsync(int userID);
    Task<bool> RevokeTokenAsync(int userID);
    Task<User> FindByEmailAsync(string email);
    Task<bool> ConfirmEmailAsync(string email, bool IsEmailConfirmed);
    Task<PaginatedList<LogResponse>> GetLogsPaginatedAsync(int page, int pageSize, LogType logType, int userID);
    Task<PaginatedList<UserPaginationResponse>> GetPaginatedAsync(int page, int pageSize);
    Task<List<string>> GetUserRolesAsync(int userID);
    Task<bool> CreateUserRoleAsync(UserRole userRole, bool saveChanges = true);
}
