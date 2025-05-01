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
    Task<bool> UpdateWithoutPassword(string Name, string OldEmail, string Email);
    Task<bool> DeleteAsync(int ID);
    Task<bool> CreateRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshToken(string token);
    Task<bool> UpdateRefreshToken(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenByUserID(int userID);
    Task<bool> RevokeToken(int userID);
    Task<CurrencyType> GetUserCurrencyAsync(int UserID);
    Task<CurrencyType> UpdateUserCurrencyAsync(int UserID, CurrencyType currencyType);
}
