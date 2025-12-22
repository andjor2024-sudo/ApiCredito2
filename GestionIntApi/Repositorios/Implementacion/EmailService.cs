using GestionIntApi.Models;
using GestionIntApi.Repositorios.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace GestionIntApi.Repositorios.Implementacion
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _settings;
        private readonly SendGridSettings _settings1;
        /*public EmailService(IOptions<SendGridSettings> options)
        {
            _settings1 = options.Value
                ?? throw new ArgumentNullException(nameof(options));
        }
        */


        // public EmailService(IOptions<SendGridSettings> settings)
        // {
        //  _settings = settings.Value;
        //}
        /*
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_settings.From);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtp = new SmtpClient(_settings.Host)
            {
                Port = _settings.Port,
                Credentials = new NetworkCredential(_settings.From, _settings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
        */


        public async Task SendEmailAsync1(string to, string subject, string body)
        {
            var client = new SendGridClient(_settings1.ApiKey);
            var from = new EmailAddress(_settings1.FromEmail, _settings1.FromName);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

            var response = await client.SendEmailAsync(msg);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                Console.WriteLine("Correo enviado correctamente.");
            else
                Console.WriteLine($"Error al enviar correo: {response.StatusCode}");
        }


        public async Task SendEmailAsync2coorectoantes(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(_settings1.ApiKey))
                throw new Exception("SendGrid ApiKey no configurada");

            var client = new SendGridClient(_settings1.ApiKey);

            var from = new EmailAddress(_settings1.FromEmail, _settings1.FromName);
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(
                from,
                toEmail,
                subject,
                body,
                body
            );

            await client.SendEmailAsync(msg);
        }


        public EmailService(IOptions<SendGridSettings> options)
        {

          
            _settings1 = options.Value
                ?? throw new ArgumentNullException(nameof(options));
            // Sobrescribimos ApiKey con la variable de entorno si existe
            var apiKeyFromEnv = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            if (!string.IsNullOrWhiteSpace(apiKeyFromEnv))
                _settings1.ApiKey = apiKeyFromEnv;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            Console.WriteLine("📧 [SENDGRID] Iniciando envío de correo...");
            Console.WriteLine($"📨 [SENDGRID] Destinatario: {to}");
            Console.WriteLine($"📝 [SENDGRID] Asunto: {subject}");

            if (string.IsNullOrWhiteSpace(_settings1.ApiKey))
            {
                Console.WriteLine("❌ [SENDGRID] ApiKey está VACÍA o NULL");
                throw new Exception("SendGrid ApiKey no configurada");
            }

            Console.WriteLine($"🔑 [SENDGRID] ApiKey detectada (length: {_settings1.ApiKey.Length})");
            Console.WriteLine($"👤 [SENDGRID] FromEmail: {_settings1.FromEmail}");
            Console.WriteLine($"🏷️ [SENDGRID] FromName: {_settings1.FromName}");

            var client = new SendGridClient(_settings1.ApiKey);
            var from = new EmailAddress(_settings1.FromEmail, _settings1.FromName);
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(
                from,
                toEmail,
                subject,
                body,
                body
            );

            Console.WriteLine("🚀 [SENDGRID] Enviando correo a SendGrid...");

            var response = await client.SendEmailAsync(msg);



            Console.WriteLine($"📡 [SENDGRID] StatusCode: {(int)response.StatusCode}");

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                Console.WriteLine("✅ [SENDGRID] Correo enviado correctamente.");
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                Console.WriteLine("❌ [SENDGRID] Error al enviar correo");
                Console.WriteLine($"📄 [SENDGRID] Response body: {responseBody}");
            }
        }

    }
}
