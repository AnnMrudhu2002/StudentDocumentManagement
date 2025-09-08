using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Services.Interface
{
    public interface IStudentProfileRepository
    {
        Task<Student?> GetStudentByUserIdAsync(string userId);
        Task<(bool Success, string Message, Student? Student)> SubmitProfileAsync(ApplicationUser user, StudentProfileDto dto);
    }
}
