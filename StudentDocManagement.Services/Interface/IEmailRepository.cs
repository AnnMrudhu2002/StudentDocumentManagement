using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Interface
{
    public interface IEmailRepository
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
