using StudentDocManagement.Entity.Models;
using StudentDocManagement.Entity.Dto;

public interface IAdminRepository
{
    // Get all students for admin to approve documents
    Task<IEnumerable<StudentWithDocsDto>> GetStudentsForApprovalAsync();

    // Get all documents uploaded by a student
    Task<IEnumerable<StudentDocumentDto>> GetDocumentsByStudentIdAsync(int studentId);

    // Approve or reject a document
    Task<bool> UpdateDocumentStatusAsync(int documentId, int statusId, string? remarks);
}
