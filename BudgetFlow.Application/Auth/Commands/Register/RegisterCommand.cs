using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using System.Text;

namespace BudgetFlow.Application.Auth.Commands.Register;
public class RegisterCommand : IRequest<Result<bool>>
{
    public UserRegisterModel User { get; set; }
    public string ClientUri { get; set; }
    public class CreateUserCommandHandler : IRequestHandler<RegisterCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly ITokenProvider tokenProvider;
        private readonly IEmailService emailService;
        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenProvider tokenProvider,
            IEmailService emailService)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.tokenProvider = tokenProvider;
            this.emailService = emailService;
        }

        public async Task<Result<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.User.Password != request.User.ConfirmPassword)
                return Result.Failure<bool>(UserErrors.PasswordsDoNotMatch);

            var userExist = await userRepository.GetByEmailAsync(request.User.Email);
            if (userExist != null)
                return Result.Failure<bool>(UserErrors.UserAlreadyExists);

            User user = new User()
            {
                Name = request.User.Name,
                Email = request.User.Email,
                PasswordHash = passwordHasher.Hash(request.User.Password)
            };

            var userID = await userRepository.CreateAsync(user);
            if (userID is 0)
                return Result.Failure<bool>(UserErrors.CreationFailed);
            else
            {
                try
                {
                    #region Send e-mail confirmation
                    var token = tokenProvider.GenerateEmailConfirmationToken(user);
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
            }

            return Result.Success(true);
        }
    }
}
