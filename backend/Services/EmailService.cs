using System.Net;
using System.Net.Mail;

namespace PortfolioAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlContent);
        Task SendContactFormEmailAsync(string name, string email, string subject, string message, string? phone = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("EmailSettings");
                var smtpHost = smtpSettings["SmtpHost"] ?? throw new InvalidOperationException("SmtpHost not configured");
                var smtpPort = int.Parse(smtpSettings["SmtpPort"] ?? "587");
                var smtpUsername = smtpSettings["SmtpUsername"] ?? throw new InvalidOperationException("SmtpUsername not configured");
                var smtpPassword = smtpSettings["SmtpPassword"] ?? throw new InvalidOperationException("SmtpPassword not configured");
                var fromEmail = smtpSettings["FromEmail"] ?? throw new InvalidOperationException("FromEmail not configured");
                var fromName = smtpSettings["FromName"] ?? "Portfolio";

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = subject,
                        Body = htmlContent,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent successfully to {toEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                throw;
            }
        }

        public async Task SendContactFormEmailAsync(string name, string email, string subject, string message, string? phone = null)
        {
            // Email to admin
            var adminEmailSettings = _configuration.GetSection("EmailSettings");
            var adminEmail = adminEmailSettings["AdminEmail"] ?? throw new InvalidOperationException("AdminEmail not configured");

            var htmlContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>New Contact Form Submission</h2>
                    <p><strong>Name:</strong> {name}</p>
                    <p><strong>Email:</strong> {email}</p>
                    {(!string.IsNullOrEmpty(phone) ? $"<p><strong>Phone:</strong> {phone}</p>" : "")}
                    <p><strong>Subject:</strong> {subject}</p>
                    <p><strong>Message:</strong></p>
                    <p>{message.Replace("\n", "<br>")}</p>
                </body>
                </html>";

            await SendEmailAsync(adminEmail, $"New Contact: {subject}", htmlContent);

            // Confirmation email to user
            var confirmationHtml = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Thank You for Contacting Us!</h2>
                    <p>Dear {name},</p>
                    <p>We have received your message and will get back to you as soon as possible.</p>
                    <p><strong>Your Message:</strong></p>
                    <p>{message.Replace("\n", "<br>")}</p>
                    <br>
                    <p>Best regards,<br>The Portfolio Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, "We received your message", confirmationHtml);
        }
    }
}
