using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocManagement.Services.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;
        private readonly IStudentProfileRepository _repo;

        public DocumentRepository(AppDbContext context, IStudentProfileRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        public async Task<(bool Success, string Message, Document? Document)> UploadDocumentAsync(
     ApplicationUser user,
     FileUploadDto fileDto)
        {
            var student = await _repo.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return (false, "Student profile not found", null);

            if (fileDto.FileStream == null || fileDto.FileStream.Length == 0)
                return (false, "No file uploaded", null);

            var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileDto.FileName);
            var filePath = Path.Combine(storagePath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileDto.FileStream.CopyToAsync(stream);
            }

            var document = new Document
            {
                StudentId = student.StudentId,
                DocumentTypeId = fileDto.DocumentTypeId,
                FileName = uniqueFileName,
                FilePath = filePath,
                StatusId = 1,
                UploadedOn = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return (true, "Document uploaded successfully", document);
        }



        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Status)
                .Include(d => d.Student)
                .FirstOrDefaultAsync(d => d.DocumentId == id);
        }

        public async Task<IEnumerable<StudentDocumentDto>> GetStudentDocumentsWithDetailsAsync(int studentId)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Status)
                .Where(d => d.StudentId == studentId)
                .Select(d => new StudentDocumentDto
                {
                    DocumentId = d.DocumentId,
                    DocumentTypeName = d.DocumentType.TypeName, // assumes you have TypeName field
                    StatusName = d.Status.StatusName,          // assumes you have StatusName field
                    Remarks = d.Remarks,
                    UploadedOn = d.UploadedOn,
                    FileName = d.FileName
                })
                .ToListAsync();
        }


        public async Task<bool> UpdateStatusAsync(int documentId, int statusId, string? remarks)
        {
            var doc = await _context.Documents.FindAsync(documentId);
            if (doc == null) return false;

            doc.StatusId = statusId;
            doc.Remarks = remarks;
            if (statusId == 2) // Example: 2 = Approved
                doc.ApprovedOn = DateTime.UtcNow;

            _context.Documents.Update(doc);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
