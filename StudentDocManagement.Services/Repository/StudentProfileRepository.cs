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

        // Get student along with related data by UserId
        public async Task<Student?> GetStudentByUserIdAsync(string userId)
        {
            return await _context.Students
                 .Include(s => s.Course)          // Include course details
                .Include(s => s.IdProofType)     // Include ID proof type
                .Include(s => s.Gender)          // Include gender
                .Include(s => s.StudentEducations)   // Include education records
                .Include(s => s.Documents)       // Include uploaded documents
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        // Submit or update student profile
        public async Task<(bool Success, string Message, Student? Student)> SubmitProfileAsync(ApplicationUser user, StudentProfileDto dto)

        {

            var existing = await GetStudentByUserIdAsync(user.Id);

            if (existing == null)
            {
                // Create new student profile
                var student = new Student
                {
                    UserId = user.Id,
                    RegisterNo = user.RegisterNo,
                    DOB = dto.DOB,
                    GenderId = dto.GenderId,
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
                    CreatedOn = DateTime.Now,
                    //IsAcknowledged = false //initially false
                };

                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();

                return (true, "Profile submitted successfully", student);
            }
            else
            {

                // Update existing profile
                existing.DOB = dto.DOB;
                existing.GenderId = dto.GenderId;
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
                existing.UpdatedOn = DateTime.Now;

                _context.Students.Update(existing);
                await _context.SaveChangesAsync();

                return (true, "Profile updated successfully", existing);
            }
        }
        // Get student by StudentId (basic info, optional course details)
        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            // Include related data if needed, e.g., Course
            return await _context.Students
                .Include(s => s.Course) // optional, only if you need CourseName
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }
        // Get all education records for a student
        public async Task<List<StudentEducation>> GetEducationByStudentIdAsync(int studentId)
        {
            return await _context.StudentEducations
                                 .Where(e => e.StudentId == studentId)
                                 .ToListAsync();
        }


        // Submit or update student education
        public async Task<(bool Success, string Message, StudentEducation? Education)>
     SubmitEducationAsync(Student student, StudentEducationDto dto)
        {
            var existing = await _context.StudentEducations
                                         .FirstOrDefaultAsync(e => e.StudentId == student.StudentId &&
                                                                   e.EducationLevel == dto.EducationLevel);

            if (existing == null)
            {
                // Add new education record
                var education = new StudentEducation
                {
                    StudentId = student.StudentId,
                    EducationLevel = dto.EducationLevel, // 10th or 12th
                    InstituteName = dto.InstituteName,
                    PassingYear = dto.PassingYear,
                    MarksPercentage = dto.MarksPercentage,
                    CreatedOn = DateTime.Now
                };

                await _context.StudentEducations.AddAsync(education);
                await _context.SaveChangesAsync();

                return (true, $"{dto.EducationLevel} education details submitted successfully", education);
            }
            else
            {
                // Update existing education record
                existing.InstituteName = dto.InstituteName;
                existing.PassingYear = dto.PassingYear;
                existing.MarksPercentage = dto.MarksPercentage;
                existing.UpdatedOn = DateTime.Now;

                _context.StudentEducations.Update(existing);
                await _context.SaveChangesAsync();

                return (true, $"{dto.EducationLevel} education details updated successfully", existing);
            }
        }


        //public async Task<IEnumerable<State>> GetAllStatesAsync()
        //{
        //    return await _context.states
        //        .OrderBy(s => s.StateName)
        //        .ToListAsync();
        //}

        // Get all states except excluded ones
        public async Task<IEnumerable<State>> GetAllStatesAsync()
        {
            var excludedIds = new List<int> { 17, 21, 27, 4, 37, 2, 33, 34, 24 };

            return await _context.states
                .Where(s => !excludedIds.Contains(s.StateId))
                .OrderBy(s => s.StateName)
                .ToListAsync();
        }

        // Get districts by state
        public async Task<IEnumerable<District>> GetDistrictsByStateIdAsync(int stateId)
        {
            return await _context.districts
                .Where(d => d.StateId == stateId)
                .OrderBy(d => d.DistrictName)
                .ToListAsync();
        }

        // Get pincodes by district
        public async Task<IEnumerable<Pincode>> GetPincodesByDistrictIdAsync(int districtId)
        {
            return await _context.pincodes
                .Where(p => p.DistrictId == districtId)
                .OrderBy(p => p.Code)
                .ToListAsync();
        }

        // Get post offices by pincode
        public async Task<IEnumerable<PostOffices>> GetPostOfficesByPincodeIdAsync(int pincodeId)
        {
            return await _context.postOffices
                .Where(po => po.PincodeId == pincodeId)
                .OrderBy(po => po.OfficeName)
                .ToListAsync();
        }

        // Acknowledge profile submission after uploading required documents
        public async Task<(bool Success, string Message)> AcknowledgeAsync(string userId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
                return (false, "Student not found");

            ////check if 3 documents are uploaded
            var docCount = await _context.Documents.CountAsync(d => d.StudentId == student.StudentId);
            if (docCount < 3)
                return (false, "Please upload all required documents before acknowledging");

            student.IsAcknowledged = true;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return (true, "Profile submitted successfully");
        }

        // Get all documents uploaded by a student
        public async Task<List<Document>> GetDocumentsByStudentIdAsync(int studentId)
        {
            return await _context.Documents
                .Include(d => d.Status)  // Include StatusMaster for checking approval
                .Where(d => d.StudentId == studentId)
                .AsNoTracking()          // Read-only query
                .ToListAsync();
        }

    }
}
