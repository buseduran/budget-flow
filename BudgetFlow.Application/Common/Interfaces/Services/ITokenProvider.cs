using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Services;
public interface ITokenProvider
{
    string Create(UserDto user);
    string GenerateRefreshToken();
}
