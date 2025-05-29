namespace BudgetFlow.Application.Common.Services.Abstract;

public interface ICacheService
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);
    bool TryGetValue<T>(string key, out T value);
}