using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocManagement.Services.Repository
{
    public class StudentProfileRepository: IStudentProfileRepository
    {
        private readonly AppDbContext _context;

        public StudentProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetStudentByUserIdAsync(string userId)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
        }


        public async Task<(bool Success, string Message, Student? Student)> SubmitProfileAsync(ApplicationUser user, StudentProfileDto dto)

        {
            var existing = await GetStudentByUserIdAsync(user.Id);
            if (existing != null)
            {
                return (false, "Profile already submitted", null);
            }

            var student = new Student
            {
                UserId = user.Id,
                RegisterNo = user.RegisterNo,
                DOB = dto.DOB,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
                AlternatePhoneNumber = dto.AlternatePhoneNumber,
                Address = dto.Address,
                PermanentAddress = dto.PermanentAddress,
                City = dto.City,
                District = dto.District,
                State = dto.State,
                Pincode = dto.Pincode,
                IdProofTypeId = dto.IdProofTypeId,
                IdProofNumber = dto.IdProofNumber,
                CourseId = dto.CourseId,
                StatusId = 1, // Pending
                CreatedOn = DateTime.UtcNow
              
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return (true, "Profile submitted successfully", student);
        }


      
    
}
}
