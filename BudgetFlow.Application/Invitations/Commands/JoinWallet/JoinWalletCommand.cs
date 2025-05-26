using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Invitations.Commands.JoinWallet;
public class JoinWalletCommand : IRequest<Result<bool>>
{
    public string Token { get; set; }

    public class JoinWalletCommandHandler : IRequestHandler<JoinWalletCommand, Result<bool>>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUserRepository _userRepository;

        public JoinWalletCommandHandler(
            IInvitationRepository invitationRepository,
            IUserWalletRepository userWalletRepository,
            IUnitOfWork unitOfWork,
            ITokenProvider tokenProvider,
            IUserRepository userRepository)
        {
            _invitationRepository = invitationRepository;
            _userWalletRepository = userWalletRepository;
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
            _userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(JoinWalletCommand request, CancellationToken cancellationToken)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(request.Token);
            if (invitation == null)
                return Result.Failure<bool>(InvitationErrors.NotFound);

            #region Davet bilgilerini doğrula.
            if (invitation.Expiration < DateTime.UtcNow)
                return Result.Failure<bool>(InvitationErrors.Expired);

            var (isValid, walletId, email) = await _tokenProvider.VerifyWalletInvitationToken(request.Token);
            if (!isValid)
                return Result.Failure<bool>(InvitationErrors.InvalidToken);

            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null || user.IsEmailConfirmed is false)
                return Result.Failure<bool>(UserErrors.UserNotFoundWithInvitation);
            #endregion

            #region Davet linkini sil ve userwallet kaydı oluştur.
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _invitationRepository.DeleteAsync(invitation.ID, saveChanges: false);

                await _userWalletRepository.CreateAsync(new UserWallet
                {
                    UserID = user.ID,
                    WalletID = walletId,
                    Role = WalletRole.Viewer
                }, saveChanges: false);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));

            }
            #endregion
        }
    }
}
