using BudgetFlow.Application.Auth;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IUserRepository
{
    Task<int> CreateAsync(User user);
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
}
