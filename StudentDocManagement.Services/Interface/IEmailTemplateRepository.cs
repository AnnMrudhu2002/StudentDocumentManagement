using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Interface
{
    public interface IEmailTemplateRepository
    {
        string GetApprovalTemplate(string fullName, string email, string registerNo);
        string GetRejectionTemplate(string fullName, string email, string registerNo);
    }
}
