using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Repository
{
    public interface IStudentRepository
    {
        Task<(bool Success, string Message, List<object> Students)> GetPendingStudentsAsync();
    }
}
