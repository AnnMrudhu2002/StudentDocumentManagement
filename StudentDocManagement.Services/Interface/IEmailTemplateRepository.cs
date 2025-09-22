namespace StudentDocManagement.Services.Interface
{
    public interface IEmailTemplateRepository
    {
        string GetApprovalTemplate(string fullName, string email, string registerNo);
        string GetRejectionTemplate(string fullName, string email, string registerNo);
    }
}
