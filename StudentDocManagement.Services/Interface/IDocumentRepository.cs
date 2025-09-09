using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Services.Interface
{
    public interface IDocumentRepository
    {
        Task<Document> UploadAsync(Document document);
        Task<Document?> GetByIdAsync(int id);
        Task<IEnumerable<StudentDocumentDto>> GetStudentDocumentsWithDetailsAsync(int studentId);

        Task<bool> UpdateStatusAsync(int documentId, int statusId, string? remarks);

    }
}
