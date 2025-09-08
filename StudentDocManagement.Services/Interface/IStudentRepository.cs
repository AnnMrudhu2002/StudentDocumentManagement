using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Interface
{
    public interface IStudentRepository
    {
        Task<(string Message, List<object> Students)> GetPendingStudentsAsync();
        Task<(bool Success, string Message)> UpdateStudentStatusAsync(string userId, int statusId);
    }
}
