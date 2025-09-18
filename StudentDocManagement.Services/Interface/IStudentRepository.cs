namespace StudentDocManagement.Services.Interface
{
    public interface IStudentRepository
    {
        Task<(string Message, List<object> Students)> GetPendingStudentsAsync();
        Task<(bool Success, string Message)> UpdateStudentStatusAsync(string userId, int statusId);
    }
}
