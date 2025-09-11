using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Services.Interface
{
    public interface IDocumentRepository
    {
        Task<(bool Success, string Message, Document? Document)> UploadDocumentAsync(ApplicationUser user, FileUploadDto fileDto);
        Task<Document?> GetByIdAsync(int id);
        Task<IEnumerable<StudentDocumentDto>> GetStudentDocumentsWithDetailsAsync(int studentId);
        //Task<IEnumerable<StudentDocumentDto>> GetStudentDocumentDetails(int studentId);

    }
}
