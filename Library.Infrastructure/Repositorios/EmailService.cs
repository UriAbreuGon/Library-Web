using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Library.Domain.Interfaces;

namespace Library.Infrastructure.Repositorios
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPortStr = _configuration["EmailSettings:SmtpPort"];
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpPortStr) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) || string.IsNullOrEmpty(fromEmail))
            {
                throw new ArgumentNullException("Los valores de configuración de SMTP no pueden ser nulos o vacíos");
            }

            var smtpPort = int.Parse(smtpPortStr);

            var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(destinatario);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Manejar el error de envío de correo
                throw new InvalidOperationException("Error enviando correo electrónico: " + ex.Message);
            }
        }
    }
}
