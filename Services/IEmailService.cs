namespace CourseDx.Services
{
    /// <summary>
    /// Interface for email service
    /// </summary>
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailAsync(string[] to, string subject, string body, bool isHtml = true);
        Task SendWelcomeEmailAsync(string to, string userName);
        Task SendEnrollmentConfirmationAsync(string to, string userName, string courseName);
        Task SendPasswordResetEmailAsync(string to, string resetLink);
    }
}
