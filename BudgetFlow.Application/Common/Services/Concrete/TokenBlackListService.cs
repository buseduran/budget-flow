using BudgetFlow.Application.Common.Services.Abstract;
using System.Collections.Concurrent;

namespace BudgetFlow.Application.Common.Services.Concrete;
public class TokenBlackListService : ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();
    public bool IsBlacklisted(string token)
    {
        if (_blacklistedTokens.TryGetValue(token, out var expiry))
        {
            if (DateTime.UtcNow < expiry)
                return true;
            _blacklistedTokens.TryRemove(token, out _); // süresi dolmuşsa sil
        }
        return false;
    }

    public void Blacklist(string token)
    {
        _blacklistedTokens[token] = DateTime.UtcNow.AddMinutes(15); // token süresi kadar tut
    }
}
