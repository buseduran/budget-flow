using BudgetFlow.Application.Common.Services.Abstract;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace BudgetFlow.Application.Common.Services.Concrete;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();
    private readonly IConfiguration _configuration;

    public TokenBlacklistService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsBlacklisted(string token)
    {
        if (_blacklistedTokens.TryGetValue(token, out var expiry))
        {
            if (DateTime.UtcNow < expiry)
                return true;

            _blacklistedTokens.TryRemove(token, out _);
        }
        return false;
    }

    public void Blacklist(string token)
    {
        var expirationMinutes = _configuration.GetValue<int>("Jwt:PasswordResetExpirationInMinutes");
        _blacklistedTokens[token] = DateTime.UtcNow.AddMinutes(expirationMinutes);
    }
}
