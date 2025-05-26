using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BudgetFlow.Application.Users.Commands.ForgotPassword;
public class ForgotPasswordCommand : IRequest<Result<bool>>
{
    public ForgotPasswordDto ForgotPassword { get; set; }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            ITokenProvider tokenProvider,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenProvider = tokenProvider;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.ForgotPassword.Email))
                return Result.Failure<bool>(UserErrors.EmailCannotBeEmpty);

            var user = await _userRepository.FindByEmailAsync(request.ForgotPassword.Email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var token = _tokenProvider.GeneratePasswordResetToken(user);

            var parameters = new Dictionary<string, string>
            {
                { "token", token },
                { "email", request.ForgotPassword.Email }
            };
            var emailConfig = _configuration.GetSection("EmailConfiguration");

            var uriBuilder = new UriBuilder(emailConfig["ForgotURI"])
            {
                Query = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"))
            };
            var resetLink = uriBuilder.ToString();

            #region Html şablonu okunur 
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Common", "Resources", "Templates", "EmailPasswordTemplate.html");
            var emailTemplate = File.ReadAllText(templatePath, Encoding.UTF8);

            var emailBody = emailTemplate.Replace("{{resetLink}}", resetLink);
            #endregion

            var emailSubject = "Şifre Sıfırlama Talebi";
            await _emailService.SendEmailAsync(request.ForgotPassword.Email, emailSubject, emailBody);

            return Result.Success(true);
        }
    }
}
