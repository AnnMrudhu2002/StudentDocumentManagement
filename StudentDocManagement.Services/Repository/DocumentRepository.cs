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
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;
        private readonly IStudentProfileRepository _repo;

        public DocumentRepository(AppDbContext context, IStudentProfileRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        public async Task<(bool Success, string Message, Document? Document)> UploadDocumentAsync(ApplicationUser user, FileUploadDto fileDto)
        {
            var student = await _repo.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return (false, "Student profile not found", null);

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

            //var allowedMimeTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
            //var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var allowedMimeTypes = new[] { "application/pdf"};
            var allowedExtensions = new[] { ".pdf" };

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
