using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Services.Interface
{
    public interface IStudentProfileRepository
    {
        Task<Student?> GetStudentByUserIdAsync(string userId);
        Task<(bool Success, string Message, Student? Student)> SubmitProfileAsync(ApplicationUser user, StudentProfileDto dto);

        Task<List<StudentEducation>> GetEducationByStudentIdAsync(int studentId);
        Task<(bool Success, string Message, StudentEducation? Education)> SubmitEducationAsync(Student student, StudentEducationDto dto);

        Task<IEnumerable<State>> GetAllStatesAsync();
        Task<IEnumerable<District>> GetDistrictsByStateIdAsync(int stateId);
        Task<IEnumerable<Pincode>> GetPincodesByDistrictIdAsync(int districtId);
        Task<IEnumerable<PostOffices>> GetPostOfficesByPincodeIdAsync(int pincodeId);
        Task<(bool Success, string Message)> AcknowledgeAsync(string userId);

        Task<List<Document>> GetDocumentsByStudentId(int studentId);
    }
}
