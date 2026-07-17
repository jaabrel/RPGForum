using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RPGForum.Services
{

    /// <summary>
    /// EmailSettings - Configuração de envio de emails. Vindo do appsettings.json
    /// </summary>
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public string SenderName { get; set; } = "RPG Forum";
    }

    /// <summary>
    /// Enviar Emails através do SMTP do GMAIL. 
    /// </summary>
    public class GmailEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public GmailEmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

                public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
 
            message.Body = new BodyBuilder
            {
                HtmlBody = htmlMessage
            }.ToMessageBody();
 
            using var client = new SmtpClient();
 
            try
            {
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
