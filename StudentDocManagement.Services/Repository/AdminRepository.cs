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

}
