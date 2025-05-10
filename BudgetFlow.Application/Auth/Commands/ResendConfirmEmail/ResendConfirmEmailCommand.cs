using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using System.Text;

namespace BudgetFlow.Application.Auth.Commands.ResendConfirmEmail;
public class ResendConfirmEmailCommand:IRequest<Result<bool>>
{
    public string Email { get; set; }
    public string ClientUri { get; set; }
    public class ResendConfirmEmailCommandHandler : IRequestHandler<ResendConfirmEmailCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenProvider tokenProvider;
        private readonly IEmailService emailService;
        public ResendConfirmEmailCommandHandler(
            IUserRepository userRepository,
            ITokenProvider tokenProvider,
            IEmailService emailService)
        {
            this.userRepository = userRepository;
            this.tokenProvider = tokenProvider;
            this.emailService = emailService;
        }
        public async Task<Result<bool>> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            #region Check User's Email is Confirmed
            if (user.IsEmailConfirmed)
                return Result.Failure<bool>(UserErrors.EmailAlreadyConfirmed);

            User userEntity = new User()
            {
                ID = user.ID,
                Name = user.Name,
                Email = user.Email,
                IsEmailConfirmed = false
            };
            #endregion

            #region Send e-mail confirmation
            try
            {
                #region Send e-mail confirmation
                var token = tokenProvider.GenerateEmailConfirmationToken(userEntity);
                var confirmationLink = $"{request.ClientUri}?token={token}&email={user.Email}";

                var templatePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Templates", "EmailConfirmTemplate.html");
                var emailTemplate = File.ReadAllText(templatePath, Encoding.UTF8);

                var emailBody = emailTemplate.Replace("{{confirmationLink}}", confirmationLink);
                var emailSubject = "E-posta Doğrulama";
                await emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                #endregion
            }
            catch (Exception)
            {
                return Result.Failure<bool>(UserErrors.EmailConfirmationMailFailed);
            }
            #endregion

            return Result.Success(true);
        }
    }
}
