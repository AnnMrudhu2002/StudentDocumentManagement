using Microsoft.AspNetCore.Http;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Services.Interface
{
    public interface IDocumentRepository
    {
        Task<(bool Success, string Message, Document? Document)> UploadDocumentAsync(ApplicationUser user, FileUploadDto fileDto);
        Task<Document?> GetByIdAsync(int id);
        Task<IEnumerable<StudentDocumentDto>> GetStudentDocumentsWithDetailsAsync(int studentId);

        Task<(byte[] FileBytes, string FileName)?> GetDocumentForDownloadAsync(int documentId, ApplicationUser user);
        Task<bool> UpdateStatusAsync(int documentId, int statusId, string? remarks);
        Task<(bool Success, string Message)> DeleteDocumentAsync(ApplicationUser user, int documentId);
        Task<bool> ReUploadDocumentAsync(Document existingDoc, IFormFile file);
    }
}
