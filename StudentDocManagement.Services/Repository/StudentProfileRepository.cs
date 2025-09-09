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

            if (existing == null)
            {
                // Create new
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
            else
            {
                // Prevent editing if admin already Approved
                if (existing.StatusId == 2)
                {
                    return (false, "Profile already approved, cannot edit", existing);
                }

                else if (existing.StatusId == 5)
                {
                    return (false, "Profile cannot be edited while under review", existing);
                }

                // Update
                existing.DOB = dto.DOB;
                existing.Gender = dto.Gender;
                existing.PhoneNumber = dto.PhoneNumber;
                existing.AlternatePhoneNumber = dto.AlternatePhoneNumber;
                existing.Address = dto.Address;
                existing.PermanentAddress = dto.PermanentAddress;
                existing.City = dto.City;
                existing.District = dto.District;
                existing.State = dto.State;
                existing.Pincode = dto.Pincode;
                existing.IdProofTypeId = dto.IdProofTypeId;
                existing.IdProofNumber = dto.IdProofNumber;
                existing.CourseId = dto.CourseId;
                existing.StatusId = 1; // Reset to pending
                existing.UpdatedOn = DateTime.UtcNow;

                _context.Students.Update(existing);
                await _context.SaveChangesAsync();

                return (true, "Profile updated successfully", existing);
            }
        }



        public async Task<StudentEducation?> GetEducationByStudentIdAsync(int studentId)
        {
            return await _context.StudentEducations
                                 .FirstOrDefaultAsync(e => e.StudentId == studentId);
        }


        public async Task<(bool Success, string Message, StudentEducation? Education)>
           SubmitEducationAsync(Student student, StudentEducationDto dto)
        {
            var existing = await GetEducationByStudentIdAsync(student.StudentId);

           
            if (student.StatusId == 2) // Approved
            {
                return (false, "Profile already approved, cannot edit education details", existing);
            }
            else if (student.StatusId == 5) // Under review
            {
                return (false, "Profile cannot be edited while under review", existing);
            }

            if (existing == null)
            {
                // Create new
                var education = new StudentEducation
                {
                    StudentId = student.StudentId,
                    EducationLevel = dto.EducationLevel,
                    InstituteName = dto.InstituteName,
                    PassingYear = dto.PassingYear,
                    MarksPercentage = dto.MarksPercentage,
                    CreatedOn = DateTime.UtcNow,
                    DocumentId = 1
                };

                _context.StudentEducations.Add(education);
                await _context.SaveChangesAsync();

                return (true, "Education details submitted successfully", education);
            }
            else
            {
               
                // Update
                existing.EducationLevel = dto.EducationLevel;
                existing.InstituteName = dto.InstituteName;
                existing.PassingYear = dto.PassingYear;
                existing.MarksPercentage = dto.MarksPercentage;
                existing.UpdatedOn = DateTime.UtcNow;

                _context.StudentEducations.Update(existing);
                await _context.SaveChangesAsync();

                return (true, "Education details updated successfully", existing);
            }
        }



    

    }
}
