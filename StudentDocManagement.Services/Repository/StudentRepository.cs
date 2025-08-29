using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentDocManagement.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Repository
{
    public class StudentRepository: IStudentRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public StudentRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        public async Task<(string Message, List<object> Students)> GetPendingStudentsAsync()
        {
            var pendingStudents = await _context.Users
                .Where(u => u.StatusId == 1) // Pending
                .Select(u => new
                {
                    FullName = u.FullName,
                    Email = u.Email,
                    RegisterNo = u.RegisterNo,
                    Id = u.Id
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

            student.StatusId = statusId;
            await _context.SaveChangesAsync();

            // Send email via EmailService
            var subject = statusId == 2
            ? "Registration Approved - Student Document Management System"
            : "Registration Rejected - Student Document Management System";

            var body = statusId == 2
                ? $@"
                <p>Dear <b>{student.FullName}</b>,</p>
                <p>We are pleased to inform you that your registration with the 
                <b>Student Document Management System</b> has been <span style='color:green;font-weight:bold;'>approved</span>.</p>
        
                <p><u>Registration Details:</u></p>
                <ul>
                    <li><b>Full Name:</b> {student.FullName}</li>
                    <li><b>Email:</b> {student.Email}</li>
                    <li><b>Register Number:</b> {student.RegisterNo}</li>
                </ul>

                <p>You can now log in to the system and access your dashboard.</p>
        
                <br/>
                <p>Best Regards,<br/>
                <b>Admin Team</b><br/>
                Student Document Management System</p>
    "
                : $@"
                <p>Dear <b>{student.FullName}</b>,</p>
                <p>We regret to inform you that your registration with the 
                <b>Student Document Management System</b> has been <span style='color:red;font-weight:bold;'>rejected</span>.</p>
        
                <p><u>Registration Details:</u></p>
                <ul>
                    <li><b>Full Name:</b> {student.FullName}</li>
                    <li><b>Email:</b> {student.Email}</li>
                    <li><b>Register Number:</b> {student.RegisterNo}</li>
                </ul>

                <p>If you believe this decision was made in error or wish to reapply, 
                kindly contact the administration office for further clarification.</p>
        
                <br/>
                <p>Best Regards,<br/>
                <b>Admin Team</b><br/>
                Student Document Management System</p>
                ";


            await _emailService.SendEmailAsync(student.Email, subject, body);

            return (true, statusId == 2 ? "Student approved" : "Student rejected");
        }
    }
}


