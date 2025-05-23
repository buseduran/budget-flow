using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BudgetFlow.Application.Users.Commands.Register;
public class RegisterCommand : IRequest<Result<bool>>
{
    public UserRegisterModel User { get; set; }
    //public string ClientUri { get; set; }
    public class CreateUserCommandHandler : IRequestHandler<RegisterCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly ITokenProvider tokenProvider;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenProvider tokenProvider,
            IEmailService emailService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.tokenProvider = tokenProvider;
            this.emailService = emailService;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
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

            await unitOfWork.BeginTransactionAsync();
            try
            {
                var userID = await userRepository.CreateAsync(user);

                #region Create UserRole
                var userRole = new UserRole()
                {
                    UserID = userID,
                    RoleID = Role.MemberID // Default role
                };
                await userRepository.CreateUserRoleAsync(userRole, saveChanges: false);
                #endregion

                #region Send e-mail confirmation
                var emailConfig = configuration.GetSection("EmailConfiguration");

                var token = tokenProvider.GenerateEmailConfirmationToken(user);
                var confirmationLink = $"{emailConfig["ConfirmURI"]}?token={token}&email={user.Email}";

                var templatePath = Path.Combine(AppContext.BaseDirectory, "Common", "Resources", "Templates", "EmailConfirmTemplate.html");
                var emailTemplate = File.ReadAllText(templatePath, Encoding.UTF8);

                var emailBody = emailTemplate.Replace("{{confirmationLink}}", confirmationLink);
                var emailSubject = "E-posta Doğrulama";
                await emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                #endregion

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return Result.Success(true);
            }
            catch (EmailSendException)
            {
                await unitOfWork.RollbackAsync();
                return Result.Failure<bool>(UserErrors.EmailConfirmationMailFailed);
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                return Result.Failure<bool>(UserErrors.CreationFailed);
            }
        }
    }
}
