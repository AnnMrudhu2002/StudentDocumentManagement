using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudentDocManagement.Services.Repository
{
    public class StudentRepository: IStudentRepository
    {
        private readonly AppDbContext _context; // EF Core DbContext for database access
        private readonly IEmailRepository _emailRepository;// Service to send emails
        private readonly IEmailTemplateRepository _emailTemplateRepository; // Service to get email templates

        public StudentRepository(AppDbContext context, IEmailRepository emailRepository, IEmailTemplateRepository emailTemplateRepository)
        {
            _context = context;
            _emailRepository = emailRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }

        // Get all students whose registration is pending
        public async Task<(string Message, List<object> Students)> GetPendingStudentsAsync()
        {
            var pendingStudents = await _context.Users
                .Where(u => u.StatusId == 1) // Pending
                .Select(u => new
                {
                    u.FullName,// Student's full name
                    u.Email,// Student's email
                    u.RegisterNo, // Student's registration number
                    u.Id  // User ID
                })
                .ToListAsync();
            // If no pending students found, return empty list with message
            if (!pendingStudents.Any())
                return ("No pending students found", new List<object>());
            // Return list of pending students
            return ("Pending students retrieved successfully", pendingStudents.Cast<object>().ToList());
        }




        // Update the status of a student (Approve or Reject) and send email notification
        public async Task<(bool Success, string Message)> UpdateStudentStatusAsync(string userId, int statusId)
       {
            // Get the student by UserId
            var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
             if (student == null)
                  return (false, "Student not found");
            // Prevent updating if student is not pending
            if (student.StatusId != 1) // pending check
                  return (false, "Student status is already updated");
            // Update student status
            student.StatusId = statusId;
             await _context.SaveChangesAsync();

             try
             {
                     string subject;
                     string body;
                // Prepare email based on new status
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
                // Send email to student
                await _emailRepository.SendEmailAsync(student.Email, subject, body);
            }
             catch (Exception ex)
             {
                // Log any email sending errors
                      Console.WriteLine("Email sending failed: " + ex.Message);
             }
            // Return result message
            return (true, statusId == 2 ? "Student approved" : "Student rejected");
       }

    }
}


