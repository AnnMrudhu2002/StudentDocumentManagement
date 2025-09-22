using StudentDocManagement.Entity.Models;
using StudentDocManagement.Entity.Dto;

public interface IAdminRepository
{
    // get all students with pending documents
    Task<IEnumerable<StudentWithDocsDto>> GetStudentsForApprovalAsync();

    // get all documents uploaded by a student
    Task<IEnumerable<StudentDocumentDto>> GetDocumentsByStudentIdAsync(int studentId);

    // approve or reject a document
    Task<bool> UpdateDocumentStatusAsync(int documentId, int statusId, string? remarks);
    Task<Student> GetStudentByIdAsync(int studentId);

}
