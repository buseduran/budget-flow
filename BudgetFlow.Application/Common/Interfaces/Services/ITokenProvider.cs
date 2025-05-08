using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Services;
public interface ITokenProvider
{
    string Create(User user);
    string GenerateRefreshToken();
    string GeneratePasswordResetToken(User user);
    bool VerifyPasswordResetToken(int userId, string token);
}
