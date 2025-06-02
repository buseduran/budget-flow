using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Services;
public interface ITokenProvider
{
    Task<string> Create(User user);
    string GenerateRefreshToken();
    string GeneratePasswordResetToken(User user);
    bool VerifyPasswordResetToken(int userId, string token);
    string GenerateEmailConfirmationToken(User user);
    bool VerifyEmailConfirmationToken(int userId, string token);
    string GenerateWalletInvitationToken(string email, int walletId);
    (bool IsValid, int WalletID, string email) VerifyWalletInvitationToken(string token);
}
