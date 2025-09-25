using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Services.Interface;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }


        // API to get all students whose registration is pending approval
        [HttpGet("GetPendingStudents")]
        public async Task<IActionResult> GetPendingStudents()
        {
            // Call repository to fetch pending students
            // The repository returns a tuple with a message and a list of students
            var (message, students) = await _studentRepository.GetPendingStudentsAsync();
            // Return the list along with the message as JSON
            return Ok(new { message, students });
        }


        // approve or reject pending students
        [HttpPatch("ApproveRejectStudents")]
        public async Task<IActionResult> UpdateStatus([FromBody] StudentStatusUpdateDto dto)
        {
            // Determine the status ID based on approval or rejection
            // 2 = Approved, 3 = Rejected
            int statusId = dto.IsApproved ? 2 : 3;
            // Call repository to update the student's status
            var (success, message) = await _studentRepository.UpdateStudentStatusAsync(dto.UserId, statusId);
            // Return success/failure and message as JSON
            return Ok(new { success, message });
        }
    }
}