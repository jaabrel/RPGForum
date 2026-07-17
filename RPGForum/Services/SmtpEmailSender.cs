using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace RPGForum.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _config["Smtp:Host"];
            if (string.IsNullOrEmpty(host))
            {
                // Ignora o envio se o SMTP não estiver configurado (ambiente local)
                return;
            }

            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("RPG Forum", _config["Smtp:From"]));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

                using var client = new SmtpClient();
                await client.ConnectAsync(host, int.Parse(_config["Smtp:Port"] ?? "25"), false);
                await client.AuthenticateAsync(_config["Smtp:From"], _config["Smtp:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch
            {
                // Logar silenciosamente ou apenas impedir o crash no fluxo de registo
            }
        }
    }
}
