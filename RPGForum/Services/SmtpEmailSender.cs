using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RPGForum.Services
{
    /// <summary>
    /// Enviar Emails através do SMTP do GMAIL. 
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

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
                var port = int.Parse(_config["Smtp:Port"] ?? "587");

                // Suporta STARTTLS (Porta 587) e SSL/TLS (Porta 465) automaticamente
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.Auto);

                await client.AuthenticateAsync(_config["Smtp:From"], _config["Smtp:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"[SMTP ERROR] Erro ao enviar email para {email}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Console.WriteLine($"[SMTP ERROR] Detalhes internos: {ex.InnerException.Message}");
                }
            }
        }
    }
}
