using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Repository
{
    public class StudentRepository: IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, List<object> Students)> GetPendingStudentsAsync()
        {
            var pendingStudents = await _context.Users
                .Where(u => u.StatusId == 1) // Pending
                .Select(u => new
                {
                    FullName = u.FullName,
                    Email = u.Email,
                    RegisterNo = u.RegisterNo
                })
                .ToListAsync();

            if (!pendingStudents.Any())
                return (false, "No pending students found.", new List<object>());

            return (true, "Pending students retrieved successfully.", pendingStudents.Cast<object>().ToList());
        }
    }
}

