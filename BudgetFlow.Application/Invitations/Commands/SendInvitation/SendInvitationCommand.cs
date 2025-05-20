using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BudgetFlow.Application.Invitations.Commands.SendInvitation;

public class SendInvitationCommand : IRequest<Result<bool>>
{
    public string Email { get; set; }
    public int WalletID { get; set; }

    public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenProvider tokenProvider;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly IInvitationRepository invitationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserWalletRepository userWalletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SendInvitationCommandHandler(
            IUserRepository userRepository,
            ITokenProvider tokenProvider,
            IEmailService emailService,
            IConfiguration configuration,
            IInvitationRepository invitationRepository,
            IUnitOfWork unitOfWork,
            IUserWalletRepository userWalletRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.tokenProvider = tokenProvider;
            this.emailService = emailService;
            this.configuration = configuration;
            this.invitationRepository = invitationRepository;
            this.unitOfWork = unitOfWork;
            this.userWalletRepository = userWalletRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<bool>> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure<bool>(UserErrors.EmailCannotBeEmpty);

            #region Wallet Kontrolü
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var wallet = await userWalletRepository.GetByWalletIdAndUserIdAsync(request.WalletID, userID);
            if (wallet is null)
                return Result.Failure<bool>(WalletErrors.WalletNotFound);
            if (wallet.Role is WalletRole.Viewer)
                return Result.Failure<bool>(WalletErrors.UserHasNoPermission);
            #endregion

            await unitOfWork.BeginTransactionAsync();
            try {
                var token = tokenProvider.GenerateWalletInvitationToken(request.Email, request.WalletID);

                var invitation = new Invitation
                {
                    Email = request.Email,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(24)
                };

                await invitationRepository.CreateAsync(invitation);

                var parameters = new Dictionary<string, string>
                {
                    { "token", token },
                    { "email", request.Email }
                };

                var emailConfig = configuration.GetSection("EmailConfiguration");

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

                await emailService.SendEmailAsync(request.Email, subject, emailBody);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                return Result.Failure<bool>(GeneralErrors.FromMessage(ex.Message));
            }
        }
    }
}
