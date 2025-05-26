using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;

namespace BudgetFlow.Application.Common.Services.Concrete;
public class WalletAuthService : IWalletAuthService
{
    private readonly IUserWalletRepository _walletUserRepository;
    public WalletAuthService(IUserWalletRepository walletUserRepository)
    {
        _walletUserRepository = walletUserRepository;
    }
    public async Task<Result<bool>> EnsureUserHasAccessAsync(int walletID, int userID, WalletRole requiredRole)
    {
        var walletUser = await _walletUserRepository.GetByWalletIdAndUserIdAsync(walletID, userID);
        if (walletUser == null)
            return Result.Failure<bool>(WalletErrors.UserNotFoundInWallet);

        if (requiredRole == WalletRole.Owner && walletUser.Role != WalletRole.Owner)
            return Result.Failure<bool>(WalletErrors.UserIsNotOwner);

        return Result.Success(true);
    }
}
