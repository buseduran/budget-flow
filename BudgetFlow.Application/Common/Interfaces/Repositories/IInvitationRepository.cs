using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces.Repositories;
public interface IInvitationRepository
{
    Task<bool> CreateAsync(Invitation invitation, bool saveChanges = true);
    Task<Invitation> GetByTokenAsync(string token);
    Task<bool> DeleteAsync(int id, bool saveChanges = true);

}
