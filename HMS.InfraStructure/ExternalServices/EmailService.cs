using HMS.Services.Abstraction;
using HMS.Shared.DTOs.MessagesDTOs;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HMS.Infrastructure.ExternalServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings , ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(EmailDTO emailDTO)
        {
            try
            {
                var mail = new MimeMessage()
                {
                    Sender = MailboxAddress.Parse(_settings.SenderEmail),
                    Subject = emailDTO.Subject,
                };
                mail.To.Add(MailboxAddress.Parse(emailDTO.EmailTo));
                mail.From.Add(new MailboxAddress(_settings.DisplayName, _settings.SenderEmail));

                var builder = new BodyBuilder();
                builder.TextBody = emailDTO.Body;
                builder.ToMessageBody();


                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_settings.Host, _settings.Port);
                await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
                await smtp.SendAsync(mail);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Error Occurred When Trying To Send Email to {emailDTO.EmailTo}", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }
}
