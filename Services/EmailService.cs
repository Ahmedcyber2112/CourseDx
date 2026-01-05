using System.Net;
using System.Net.Mail;

namespace CourseDx.Services
{
    /// <summary>
    /// Email service implementation using SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            await SendEmailAsync(new[] { to }, subject, body, isHtml);
        }

        public async Task SendEmailAsync(string[] to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "";
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@coursedx.com";
                var fromName = _configuration["EmailSettings:FromName"] ?? "CourseDx";

                if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email settings not configured. Email not sent to: {Recipients}", string.Join(", ", to));
                    return;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                foreach (var recipient in to)
                {
                    message.To.Add(recipient);
                }

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to: {Recipients}", string.Join(", ", to));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {Recipients}", string.Join(", ", to));
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string to, string userName)
        {
            var subject = "Welcome to CourseDx!";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h1 style='color: #f59e0b;'>Welcome to CourseDx!</h1>
                    <p>Hi {userName},</p>
                    <p>Thank you for joining CourseDx. We're excited to have you on board!</p>
                    <p>Start exploring our courses and begin your learning journey today.</p>
                    <br/>
                    <p>Best regards,<br/>The CourseDx Team</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendEnrollmentConfirmationAsync(string to, string userName, string courseName)
        {
            var subject = $"Enrollment Confirmed - {courseName}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h1 style='color: #f59e0b;'>Enrollment Confirmed!</h1>
                    <p>Hi {userName},</p>
                    <p>Congratulations! You have successfully enrolled in <strong>{courseName}</strong>.</p>
                    <p>You can now access your course materials from your dashboard.</p>
                    <br/>
                    <p>Happy Learning!<br/>The CourseDx Team</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var subject = "Password Reset Request - CourseDx";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h1 style='color: #f59e0b;'>Password Reset Request</h1>
                    <p>You have requested to reset your password.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}' style='background-color: #f59e0b; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <br/>
                    <p>Best regards,<br/>The CourseDx Team</p>
                </body>
                </html>";

            await SendEmailAsync(to, subject, body);
        }
    }
}
