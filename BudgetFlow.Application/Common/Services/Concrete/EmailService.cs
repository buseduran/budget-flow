using BudgetFlow.Application.Common.Services.Abstract;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace BudgetFlow.Application.Common.Services.Concrete;
public class EmailService : IEmailService
{
    private readonly IConfiguration configuration;
    public EmailService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var emailConfig = configuration.GetSection("EmailConfiguration");

        var smtpClient = new SmtpClient(emailConfig["SmtpServer"])
        {
            Port = int.Parse(emailConfig["Port"]!),
            Credentials = new NetworkCredential(
                emailConfig["Username"],
                emailConfig["Password"]
            ),
            EnableSsl = true
        };

        var fromEmail = emailConfig["From"];

        var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail!),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);

    }
}

