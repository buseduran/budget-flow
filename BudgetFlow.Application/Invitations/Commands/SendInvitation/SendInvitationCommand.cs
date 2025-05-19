using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using MediatR;

namespace BudgetFlow.Application.Invitations.Commands.SendInvitation;
public class SendInvitationCommand : IRequest<Result<bool>>
{
    public string Email { get; set; }
    public int WalletId { get; set; }
    public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, Result<bool>>
    {
        private readonly IInvitationRepository invitationRepository;
        private readonly IEmailService emailService;
        public SendInvitationCommandHandler(
            IInvitationRepository invitationRepository,
            IEmailService emailService)
        {
            this.invitationRepository = invitationRepository;
            this.emailService = emailService;
        }

        public Task<Result<bool>> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
