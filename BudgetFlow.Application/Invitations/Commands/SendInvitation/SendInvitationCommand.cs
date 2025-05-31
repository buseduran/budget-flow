using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BudgetFlow.Application.Invitations.Commands.SendInvitation;

public class SendInvitationCommand : IRequest<Result<bool>>
{
    public string Email { get; set; }
    public int WalletID { get; set; }

    public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserWalletRepository _userWalletRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWalletAuthService _walletAuthService;

        public SendInvitationCommandHandler(
            IUserRepository userRepository,
            ITokenProvider tokenProvider,
            IEmailService emailService,
            IConfiguration configuration,
            IInvitationRepository invitationRepository,
            IUnitOfWork unitOfWork,
            IUserWalletRepository userWalletRepository,
            ICurrentUserService currentUserService,
            IWalletAuthService walletAuthService)
        {
            _userRepository = userRepository;
            _tokenProvider = tokenProvider;
            _emailService = emailService;
            _configuration = configuration;
            _invitationRepository = invitationRepository;
            _unitOfWork = unitOfWork;
            _userWalletRepository = userWalletRepository;
            _currentUserService = currentUserService;
            _walletAuthService = walletAuthService;
        }

        public async Task<Result<bool>> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure<bool>(UserErrors.EmailCannotBeEmpty);

            // Ensure user has owner role
            var authResult = await _walletAuthService.EnsureUserHasAccessAsync(request.WalletID, _currentUserService.GetCurrentUserID(), WalletRole.Owner);
            if (!authResult.IsSuccess)
                return Result.Failure<bool>(authResult.Error);

            #region Wallet Kontrolü
            var userID = _currentUserService.GetCurrentUserID();
            var wallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);
            if (wallet.Role is WalletRole.Viewer)
                return Result.Failure<bool>(WalletErrors.UserHasNoPermission);
            #endregion

            #region Ensure target user is registered
            var targetUser = await _userRepository.GetByEmailAsync(request.Email);
            if (targetUser is null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var targetUserWallet = await _userWalletRepository.GetByWalletIdAndUserIdAsync(wallet.WalletID, targetUser.ID);
            if (targetUser is not null)
                return Result.Failure<bool>(WalletErrors.UserWalletAlreadyJoined);
            #endregion

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var token = _tokenProvider.GenerateWalletInvitationToken(request.Email, request.WalletID);

                var invitation = new Invitation
                {
                    Email = request.Email,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(24)
                };

                await _invitationRepository.CreateAsync(invitation);

                var parameters = new Dictionary<string, string>
                {
                    { "token", token },
                    { "email", request.Email }
                };

                var emailConfig = _configuration.GetSection("EmailConfiguration");

                var uriBuilder = new UriBuilder(emailConfig["InvitationURI"])
                {
                    Query = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"))
                };

                var invitationLink = uriBuilder.ToString();

                // HTML şablonunu oku
                var templatePath = Path.Combine(AppContext.BaseDirectory, "Common", "Resources", "Templates", "InvitationTemplate.html");
                var emailTemplate = File.ReadAllText(templatePath, Encoding.UTF8);

                var emailBody = emailTemplate.Replace("{{invitationLink}}", invitationLink);

                var subject = "Cüzdan Daveti";

                await _emailService.SendEmailAsync(request.Email, subject, emailBody);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
        }
    }
}
