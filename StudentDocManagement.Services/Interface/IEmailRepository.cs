namespace StudentDocManagement.Services.Interface
{
    public interface IEmailRepository
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
