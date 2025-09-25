using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Entity.Dto;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    // Get all students who have pending documents

    public async Task<IEnumerable<StudentWithDocsDto>> GetStudentsForApprovalAsync()
    {
        return await _context.Students
            .Include(s => s.User)// Include related User data
            .Where(s => s.Documents.Any(d => d.StatusId == 1)) // Only students with pending documents
            .Select(s => new StudentWithDocsDto
            {
                StudentId = s.StudentId,
                FullName = s.User!.FullName,
                RegisterNo = s.User.RegisterNo
            })
            .ToListAsync(); // Execute query asynchronously
    }

    // Get pending documents of a student
    public async Task<IEnumerable<StudentDocumentDto>> GetDocumentsByStudentIdAsync(int studentId)
    {
        return await _context.Documents
            .Include(d => d.DocumentType)// Include document type
            .Include(d => d.Status) // Include status
            .Where(d => d.StudentId == studentId && d.StatusId == 1) // 1 = Pending
            .Select(d => new StudentDocumentDto
            {
                DocumentId = d.DocumentId,
                DocumentTypeName = d.DocumentType!.TypeName,
                StatusName = d.Status!.StatusName,
                Remarks = d.Remarks,
            })
            .ToListAsync();
    }

    // Update document status (approve/reject) and add remarks
    public async Task<bool> UpdateDocumentStatusAsync(int documentId, int statusId, string? remarks)
    {
        var doc = await _context.Documents.FindAsync(documentId);
        if (doc == null) return false;

        doc.StatusId = statusId;//update status
        if (statusId == 3)// If rejected
        {
            doc.Remarks = remarks;// Add remarks
        }

        if (statusId == 2)// If approved
        {
            doc.Remarks = remarks;// Add remarks
            doc.ApprovedOn = DateTime.Now;// Set approved date
        }
        _context.Documents.Update(doc);
        await _context.SaveChangesAsync();

        return true;
    }
    // Get student details by ID
    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        // Include related data if needed, e.g., Course
        return await _context.Students
            .Include(s => s.Course)// Include course details
             .Include(s => s.Gender) // Include gender details
            .FirstOrDefaultAsync(s => s.StudentId == studentId);// Find student by ID
    }
    // Get education details of a student
    public async Task<List<StudentEducationDto>> GetStudentEducationsAsync(int studentId)
    {
        return await _context.StudentEducations
            .Where(e => e.StudentId == studentId)// Filter by student ID
            .Select(e => new StudentEducationDto
            {
                EducationLevel = e.EducationLevel,
                InstituteName = e.InstituteName,
                PassingYear = e.PassingYear,
                MarksPercentage = e.MarksPercentage
            })
            .ToListAsync();
    }

    // Get filtered students based on branch, name, register number
    public async Task<List<StudentListDto>> GetFilteredStudentsAsync(string branch, string name, string registerNo)
    {
        var query = _context.Students
            .Include(s => s.Course)
            .Include(s => s.Status)
             .Include(s => s.Documents)
            .Include(s => s.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(branch))
            query = query.Where(s => s.Course != null && s.Course.CourseName.Contains(branch));// Filter by branch

        if (!string.IsNullOrEmpty(name))
            query = query.Where(s => s.User != null && s.User.FullName.Contains(name));// Filter by name

        if (!string.IsNullOrEmpty(registerNo))
            query = query.Where(s => s.User != null && s.User.RegisterNo.Contains(registerNo));// Filter by regNo


        return await query.Select(s => new StudentListDto
        {
            StudentId = s.StudentId,
            FullName = s.User!.FullName,
            RegisterNo = s.User.RegisterNo,
            Branch = s.Course != null ? s.Course.CourseName : "", // Branch name
            Email = s.User.Email
        }).ToListAsync();
    }


    // Get full student profile including education
    public async Task<ProfilePageDto?> GetStudentProfileAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Course)// Include course
            .Include(s => s.Status)// Include status
            .Include(s => s.Gender)// Include gender
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student == null) return null;
        // Get education list for student
        var educations = await _context.StudentEducations
            .Where(e => e.StudentId == studentId)
            .Select(e => new StudentEducationDto
            {
                EducationLevel = e.EducationLevel,
                InstituteName = e.InstituteName,
                PassingYear = e.PassingYear,
                MarksPercentage = e.MarksPercentage
            }).ToListAsync();
        // Return profile DTO
        return new ProfilePageDto
        {
            FullName = student.User!.FullName,
            Email = student.User.Email,
            DOB = student.DOB,
            GenderId = student.GenderId,
            GenderName = student.Gender?.Name ?? "",
            PhoneNumber = student.PhoneNumber,
            AlternatePhoneNumber = student.AlternatePhoneNumber,
            Address = student.Address,
            PermanentAddress = student.PermanentAddress,
            City = student.City,
            District = student.District,
            State = student.State,
            Pincode = student.Pincode,
            IdProofTypeId = student.IdProofTypeId,
            IdProofNumber = student.IdProofNumber,
            CourseId = student.CourseId,
            CourseName = student.Course?.CourseName ?? "",
            Educations = educations
        };
    }


}
