using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Invitations.Commands.JoinWallet;
public class JoinWalletCommand : IRequest<Result<bool>>
{
    public string Token { get; set; }
    //public int WalletID { get; set; }

    public class JoinWalletCommandHandler : IRequestHandler<JoinWalletCommand, Result<bool>>
    {
        private readonly IInvitationRepository invitationRepository;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUnitOfWork unitOfWork;
        private readonly ITokenProvider tokenProvider;
        private readonly IUserRepository userRepository;

        public JoinWalletCommandHandler(
            IInvitationRepository invitationRepository,
            IUserWalletRepository userWalletRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            ITokenProvider tokenProvider,
            IUserRepository userRepository)
        {
            this.invitationRepository = invitationRepository;
            this.userWalletRepository = userWalletRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
            this.tokenProvider = tokenProvider;
            this.userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(JoinWalletCommand request, CancellationToken cancellationToken)
        {
            var invitation = await invitationRepository.GetByTokenAsync(request.Token);
            if (invitation == null)
                return Result.Failure<bool>(InvitationErrors.NotFound);

            #region Davet bilgilerini doğrula.
            if (invitation.Expiration < DateTime.UtcNow)
                return Result.Failure<bool>(InvitationErrors.Expired);

            var (isValid, walletId, email) = await tokenProvider.VerifyWalletInvitationToken(request.Token);
            if (!isValid)
                return Result.Failure<bool>(InvitationErrors.InvalidToken);

            var user = await userRepository.GetByEmailAsync(email);
            if (user is null || user.IsEmailConfirmed is false)
                return Result.Failure<bool>(UserErrors.UserNotFoundWithInvitation);
            #endregion

            #region Davet linkini sil ve userwallet kaydı oluştur.
            await unitOfWork.BeginTransactionAsync();
            try
            {
                await invitationRepository.DeleteAsync(invitation.ID, saveChanges: false);

                await userWalletRepository.CreateAsync(new UserWallet
                {
                    UserID = user.ID,
                    WalletID = walletId,
                    Role = WalletRole.Viewer
                }, saveChanges: false);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));

            }
            #endregion
        }
    }
}
