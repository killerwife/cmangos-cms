using Common;
using Data.Config;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private IOptionsMonitor<EmailConfig> _emailConfig;

        public EmailService(ILogger<EmailService> logger, IOptionsMonitor<EmailConfig> emailConfig)
        {
            _logger = logger;
            _emailConfig = emailConfig;
        }

        public async Task<IActionResult?> SendToken(string username, string email, string verificationToken, string caleeUrl, string locale, Operation operation)
        {
            try
            {                
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailConfig.CurrentValue.SenderAlias, _emailConfig.CurrentValue.SenderEmail));
                message.To.Add(new MailboxAddress(username, email));

                var builder = new BodyBuilder();
                switch (operation)
                {
                    case Operation.SendConfirmationEmail:
                        message.Subject = "Email verification";
                        builder.TextBody = "Please click to verify your email: " + caleeUrl + "/verifyemail/" + verificationToken;
                        message.Body = builder.ToMessageBody();
                        break;
                    default: break;
                }
            
                using var client = new SmtpClient();
                await client.ConnectAsync(_emailConfig.CurrentValue.Host, _emailConfig.CurrentValue.Port);
                await client.AuthenticateAsync(_emailConfig.CurrentValue.Email, _emailConfig.CurrentValue.Password);
                await client.SendAsync(message);
                client.Dispose();
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                return new BadRequestObjectResult("Failed to send email");
            }
        }
    }
}
