using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Auth.Commands.ForgotPassword;
public class ForgotPasswordCommand : IRequest<Result<bool>>
{
    public ForgotPasswordDto ForgotPassword { get; set; }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenProvider tokenProvider;
        private readonly IEmailService emailService;
        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            ITokenProvider tokenProvider,
            IEmailService emailService)
        {
            this.userRepository = userRepository;
            this.tokenProvider = tokenProvider;
            this.emailService = emailService;
        }

        public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.ForgotPassword.ClientUri))
                return Result.Failure<bool>(UserErrors.UserNotFound);

            if (string.IsNullOrWhiteSpace(request.ForgotPassword.Email))
                return Result.Failure<bool>(UserErrors.EmailCannotBeEmpty);

            var user = await userRepository.FindByEmailAsync(request.ForgotPassword.Email);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            var token = tokenProvider.GeneratePasswordResetToken(user);

            var parameters = new Dictionary<string, string>
            {
                { "token", token },
                { "email", request.ForgotPassword.Email }
            };
            var uriBuilder = new UriBuilder(request.ForgotPassword.ClientUri)
            {
                Query = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"))
            };
            var resetLink = uriBuilder.ToString();

            var emailBody = $"<p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p><p><a href='{resetLink}'>Şifre Sıfırlama Bağlantısı</a></p>";
            var emailSubject = "Şifre Sıfırlama Talebi";
            await emailService.SendEmailAsync(request.ForgotPassword.Email, emailSubject, emailBody);

            return Result.Success(true);
        }
    }
}
