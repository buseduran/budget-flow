using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BudgetFlow.Infrastructure.Seed;
public class AdminUserSeeder
{
    private readonly BudgetContext _context;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AdminUserSeeder(
        BudgetContext context,
        IEmailService emailService,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _context = context;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }
    public async Task SeedAsync()
    {
        var emailConfig = _configuration.GetSection("EmailConfiguration");
        var adminMail = emailConfig["From"];

        if (string.IsNullOrEmpty(adminMail))
        {
            throw new InvalidOperationException("Admin email is not set in environment variables.");
        }
        var adminUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == adminMail);

        if (adminUser == null)
        {
            var password = GenerateRandomPassword();

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                adminUser = new User
                {
                    Name = adminMail,
                    Email = adminMail,
                    IsEmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = _passwordHasher.Hash(password),
                };
                var userID = await _userRepository.CreateAsync(adminUser);

                var userRole = new UserRole()
                {
                    UserID = userID,
                    RoleID = Role.AdminID
                };
                await _userRepository.CreateUserRoleAsync(userRole, saveChanges: false);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new InvalidOperationException("Admin user creation failed.", ex);
            }

            #region Html şablonu okunur ve şifre bilgisi gönderilir
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Common", "Resources", "Templates", "AdminCredentialTemplate.html");
            var emailTemplate = File.ReadAllText(templatePath, Encoding.UTF8);

            var emailBody = emailTemplate
                .Replace("{{adminEmail}}", adminMail)
                .Replace("{{adminPassword}}", password);

            var emailSubject = "Şifre Bilgilendirme";

            await _emailService.SendEmailAsync(adminMail, emailSubject, emailBody);
            #endregion
        }
    }

    private string GenerateRandomPassword(int length = 12)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}
