using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocManagement.Services.Repository
{
    public class StudentRepository: IStudentRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public StudentRepository(AppDbContext context, IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            _context = context;
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }


        public async Task<(string Message, List<object> Students)> GetPendingStudentsAsync()
        {
            var pendingStudents = await _context.Users
                .Where(u => u.StatusId == 1) // Pending
                .Select(u => new
                {
                    u.FullName,
                    u.Email,
                    u.RegisterNo,
                    u.Id
                })
                .ToListAsync();

            if (!pendingStudents.Any())
                return ("No pending students found", new List<object>());

            return ("Pending students retrieved successfully", pendingStudents.Cast<object>().ToList());
        }





       public async Task<(bool Success, string Message)> UpdateStudentStatusAsync(string userId, int statusId)
       {
             var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
             if (student == null)
                  return (false, "Student not found");

             if (student.StatusId != 1) // pending check
                  return (false, "Student status is already updated");

                  student.StatusId = statusId;
             await _context.SaveChangesAsync();

             try
             {
                     string subject;
                     string body;

                    if (statusId == 2) // Approved
                    {
                            subject = "Registration Approved - Student Document Management System";
                            body = _emailTemplateRepository.GetApprovalTemplate(student.FullName, student.Email, student.RegisterNo);
                    }
                    else // Rejected
                    {
                             subject = "Registration Rejected - Student Document Management System";
                             body = _emailTemplateRepository.GetRejectionTemplate(student.FullName, student.Email, student.RegisterNo);
                    }

                      await _emailRepository.SendEmailAsync(student.Email, subject, body);
             }
             catch (Exception ex)
             {
                      Console.WriteLine("Email sending failed: " + ex.Message);
             }

              return (true, statusId == 2 ? "Student approved" : "Student rejected");
       }

    }
}


