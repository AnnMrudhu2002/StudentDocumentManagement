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

    public async Task<IEnumerable<StudentWithDocsDto>> GetStudentsForApprovalAsync()
    {
        return await _context.Students
            .Include(s => s.User)
            .Where(s => s.Documents.Any(d => d.StatusId == 1)) // Only students with pending documents
            .Select(s => new StudentWithDocsDto
            {
                StudentId = s.StudentId,
                FullName = s.User!.FullName,
                RegisterNo = s.User.RegisterNo
            })
            .ToListAsync();
    }


    public async Task<IEnumerable<StudentDocumentDto>> GetDocumentsByStudentIdAsync(int studentId)
    {
        return await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.Status)
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


    public async Task<bool> UpdateDocumentStatusAsync(int documentId, int statusId, string? remarks)
    {
        var doc = await _context.Documents.FindAsync(documentId);
        if (doc == null) return false;

        doc.StatusId = statusId;
        if (statusId == 3)
        {
            doc.Remarks = remarks;
        }

        if (statusId == 2)
        {
            doc.Remarks = remarks;
            doc.ApprovedOn = DateTime.Now;
        }
        _context.Documents.Update(doc);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<Student> GetStudentByIdAsync(int studentId)
    {
        // Include related data if needed, e.g., Course
        return await _context.Students
            .Include(s => s.Course)
             .Include(s => s.Gender)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }
    public async Task<List<StudentEducationDto>> GetStudentEducationsAsync(int studentId)
    {
        return await _context.StudentEducations
            .Where(e => e.StudentId == studentId)
            .Select(e => new StudentEducationDto
            {
                EducationLevel = e.EducationLevel,
                InstituteName = e.InstituteName,
                PassingYear = e.PassingYear,
                MarksPercentage = e.MarksPercentage
            })
            .ToListAsync();
    }

    public async Task<List<StudentListDto>> GetFilteredStudentsAsync(string branch, string name, string registerNo)
    {
        var query = _context.Students
            .Include(s => s.Course)
            .Include(s => s.Status)
             .Include(s => s.Documents)
            .Include(s => s.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(branch))
            query = query.Where(s => s.Course != null && s.Course.CourseName.Contains(branch));

        if (!string.IsNullOrEmpty(name))
            query = query.Where(s => s.User != null && s.User.FullName.Contains(name));

        if (!string.IsNullOrEmpty(registerNo))
            query = query.Where(s => s.User != null && s.User.RegisterNo.Contains(registerNo));


        return await query.Select(s => new StudentListDto
        {
            StudentId = s.StudentId,
            FullName = s.User!.FullName,
            RegisterNo = s.RegisterNo,
            Branch = s.Course != null ? s.Course.CourseName : "",
            Email = s.User.Email
        }).ToListAsync();
    }


    public async Task<ProfilePageDto?> GetStudentProfileAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Course)
            .Include(s => s.Status)
            .Include(s => s.Gender)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student == null) return null;

        var educations = await _context.StudentEducations
            .Where(e => e.StudentId == studentId)
            .Select(e => new StudentEducationDto
            {
                EducationLevel = e.EducationLevel,
                InstituteName = e.InstituteName,
                PassingYear = e.PassingYear,
                MarksPercentage = e.MarksPercentage
            }).ToListAsync();

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
