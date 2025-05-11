namespace BudgetFlow.Application.Common.Services.Abstract;
public interface ITokenBlacklistService
{
    bool IsBlacklisted(string token);
    void Blacklist(string token);
}
