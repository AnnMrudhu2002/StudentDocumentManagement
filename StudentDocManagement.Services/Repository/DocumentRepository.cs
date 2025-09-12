using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using StudentDocManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentRepository(AppDbContext context, IStudentProfileRepository studentProfileRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _studentProfileRepository = studentProfileRepository;
            _userManager = userManager;
        }

        public async Task<(bool Success, string Message, Document? Document)> UploadDocumentAsync(ApplicationUser user, FileUploadDto fileDto)
        {
            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return (false, "Student profile not found", null);

            // Check if this document type already uploaded by the student
            var existingDoc = await _context.Documents
                .FirstOrDefaultAsync(d => d.StudentId == student.StudentId && d.DocumentTypeId == fileDto.DocumentTypeId);

            if (existingDoc != null)
            {
                return (false, "You have already uploaded the document", null);
            }

            // Validate file
            var validationMessage = ValidateFile(fileDto);
            if (validationMessage != null)
                return (false, validationMessage, null);

            // Save to disk
            var (fileSaved, filePath, uniqueFileName, error) = await SaveFileToDiskAsync(fileDto);
            if (!fileSaved)
                return (false, error!, null);

            // Create entity
            var document = new Document
            {
                StudentId = student.StudentId,
                DocumentTypeId = fileDto.DocumentTypeId,
                FileName = uniqueFileName,
                FilePath = filePath,
                StatusId = 1, // Pending
                UploadedOn = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return (true, "Document uploaded successfully", document);
        }


        public string? ValidateFile(FileUploadDto fileDto)
        {
            //if (fileDto.FileStream == null || fileDto.FileSize == 0)
            //    return "No file uploaded";

            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (fileDto.FileSize > maxFileSize)
                return "File size exceeds the 5 MB limit";

            var allowedMimeTypes = new[] { "application/pdf", "image/jpg", "image/jpeg", "image/png" };
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            if (!allowedMimeTypes.Contains(fileDto.ContentType.ToLower()) &&
                !allowedExtensions.Contains(Path.GetExtension(fileDto.FileName).ToLower()))
            {
                return "Invalid file format. Only PDF, JPG, and PNG are allowed.";
            }

            return null; // valid
        }

        public async Task<(bool Success, string FilePath, string FileName, string? Error)> SaveFileToDiskAsync(FileUploadDto fileDto)
        {
            try
            {
                var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");
                if (!Directory.Exists(storagePath))
                    Directory.CreateDirectory(storagePath);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileDto.FileName);
                var filePath = Path.Combine(storagePath, uniqueFileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await fileDto.FileStream.CopyToAsync(stream);

                return (true, filePath, uniqueFileName, null);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, string.Empty, $"Error saving file: {ex.Message}");
            }
        }


        //download
        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Status)
                .Include(d => d.Student)
                .FirstOrDefaultAsync(d => d.DocumentId == id);
        }


        //my documents
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
                    FileName = d.FileName,
                    DocumentTypeId = d.DocumentTypeId
                })
                .ToListAsync();
        }


        public async Task<(byte[] FileBytes, string FileName)?> GetDocumentForDownloadAsync(
           int documentId, ApplicationUser user)
        {
            var doc = await GetByIdAsync(documentId);
            if (doc == null || !File.Exists(doc.FilePath))
                return null;

            // Admin can download any document
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var bytes = await File.ReadAllBytesAsync(doc.FilePath);
                return (bytes, doc.FileName);
            }

            // Check if student owns the document
            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null || doc.StudentId != student.StudentId)
                return null;

            var fileBytes = await File.ReadAllBytesAsync(doc.FilePath);
            return (fileBytes, doc.FileName);
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

        ////list uploaded files
        //public async Task<IEnumerable<StudentDocumentDto>> GetStudentDocumentDetails(int studentId)
        //{
        //    return await _context.Documents
        //        .Include(d => d.DocumentType)
        //        .Include(d => d.Status)
        //        .Where(d => d.StudentId == studentId && studentId != 3)
        //        .Select(d => new StudentDocumentDto
        //        {
        //            DocumentId = d.DocumentId,
        //            DocumentTypeName = d.DocumentType.TypeName, // assumes you have TypeName field
        //            StatusName = d.Status.StatusName,          // assumes you have StatusName field
        //            Remarks = d.Remarks,
        //            UploadedOn = d.UploadedOn,
        //            FileName = d.FileName
        //        })
        //        .ToListAsync();
        //}

        //}


        public async Task<(bool Success, string Message)> DeleteDocumentAsync(ApplicationUser user, int documentId)
        {
            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return (false, "Student profile not found");

            // Find document for this student
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.DocumentId == documentId && d.StudentId == student.StudentId);

            if (document == null)
                return (false, "Document not found or you don't have permission to delete it");

            //  Prevent deletion if status is Approved (2) or Rejected (3)
            if (document.StatusId == 2)
                return (false, "Approved documents cannot be deleted");
            if (document.StatusId == 3)
                return (false, "Rejected documents cannot be deleted");

            try
            {
                // Delete file from disk if exists
                if (File.Exists(document.FilePath))
                {
                    File.Delete(document.FilePath);
                }

                // Delete from DB
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();

                return (true, "Document deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting document: {ex.Message}");
            }
        }


    }
}
