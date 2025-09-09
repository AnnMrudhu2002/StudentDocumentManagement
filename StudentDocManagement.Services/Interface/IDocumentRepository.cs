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
        Task<(bool Success, string Message, Document? Document)> UploadDocumentAsync(ApplicationUser user,FileUploadDto fileDto);

        string ValidateFile(FileUploadDto fileDto);
        Task<(bool Success, string FilePath, string FileName, string? Error)> SaveFileToDiskAsync(FileUploadDto fileDto);
        //Task<IEnumerable<StudentDocumentDto>> GetStudentDocumentsWithDetailsAsync(int studentId);

        Task<bool> UpdateStatusAsync(int documentId, int statusId, string? remarks);

    }
}
