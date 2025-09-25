using Microsoft.Extensions.Configuration;
using StudentDocManagement.Services.Interface;
using System.Net;
using System.Net.Mail;

namespace StudentDocManagement.Services.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _configuration;

        public EmailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // Method to send an email asynchronously
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Read SMTP settings from appsettings.json
            var smtpHost = _configuration["EmailSettings:SmtpServer"]; // SMTP server address
            var smtpPort = int.Parse(_configuration["EmailSettings:Port"]); // SMTP port (usually 587 or 465)
            var fromEmail = _configuration["EmailSettings:FromEmail"]; // Sender email
            var fromPassword = _configuration["EmailSettings:Password"]; // Sender email password

            // Create and configure SMTP client
            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(fromEmail, fromPassword);
                client.EnableSsl = true;
                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail), // Sender
                    Subject = subject, // Email subject
                    Body = body, // Email body content
                    IsBodyHtml = true // Allow HTML content
                };
                mailMessage.To.Add(toEmail);// Add recipient

                // Send email asynchronously
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
