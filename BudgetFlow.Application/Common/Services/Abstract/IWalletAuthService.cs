using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Enums;

namespace BudgetFlow.Application.Common.Services.Abstract;
public interface IWalletAuthService
{
    Task<Result<bool>> EnsureUserHasAccessAsync(int walletID, int userID, WalletRole requiredRole);
}
