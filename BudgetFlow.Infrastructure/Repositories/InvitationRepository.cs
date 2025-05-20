using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Repositories;
public class InvitationRepository : IInvitationRepository
{
    private readonly BudgetContext context;
    public InvitationRepository(BudgetContext context)
    {
        this.context = context;
    }

    public async Task<bool> CreateAsync(Invitation invitation, bool saveChanges = true)
    {
        invitation.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        invitation.CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        invitation.Expiration = DateTime.SpecifyKind(invitation.Expiration, DateTimeKind.Utc);

        await context.Invitations.AddAsync(invitation);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }

    public async Task<Invitation> GetByTokenAsync(string token)
    {
        var invitation = await context.Invitations
            .FirstOrDefaultAsync(i => i.Token == token);

        return invitation;
    }
    public async Task<bool> DeleteAsync(int id, bool saveChanges = true)
    {
        var invitation = await context.Invitations
            .FirstOrDefaultAsync(i => i.ID == id);
        if (invitation == null)
            return false;
        context.Invitations.Remove(invitation);
        if (saveChanges)
            await context.SaveChangesAsync();
        return true;
    }
}
