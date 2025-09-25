using StudentDocManagement.Services.Interface;

namespace StudentDocManagement.Services.Repository
{
    public class EmailTemplateRepository: IEmailTemplateRepository
    {
        // Method to generate HTML email template for approved student registration
        public string GetApprovalTemplate(string fullName, string email, string registerNo)
        {
            // Return a formatted HTML string with placeholders filled
            return $@"
        <p>Dear <b>{fullName}</b>,</p>
        <p>We are pleased to inform you that your registration with the 
        <b>Student Document Management System</b> has been <span style='color:green;font-weight:bold;'>approved</span>.</p>

        <p><u>Registration Details:</u></p>
        <ul>
            <li><b>Full Name:</b> {fullName}</li>
            <li><b>Email:</b> {email}</li>
            <li><b>Register Number:</b> {registerNo}</li>
        </ul>

        <p>You can now log in to the system and access your dashboard.</p>

        <br/>
        <p>Best Regards,<br/>
        <b>Admin Team</b><br/>
        Student Document Management System</p>";
        }

        // Method to generate HTML email template for rejected student registration
        public string GetRejectionTemplate(string fullName, string email, string registerNo)
        {
            // Return a formatted HTML string indicating rejection
            return $@"
        <p>Dear <b>{fullName}</b>,</p>
        <p>We regret to inform you that your registration with the 
        <b>Student Document Management System</b> has been <span style='color:red;font-weight:bold;'>rejected</span>.</p>

        <p><u>Registration Details:</u></p>
        <ul>
            <li><b>Full Name:</b> {fullName}</li>
            <li><b>Email:</b> {email}</li>
            <li><b>Register Number:</b> {registerNo}</li>
        </ul>

        <p>If you believe this decision was made in error or wish to reapply, 
        kindly contact the administration office for further clarification.</p>

        <br/>
        <p>Best Regards,<br/>
        <b>Admin Team</b><br/>
        Student Document Management System</p>";
        }
    }
}
