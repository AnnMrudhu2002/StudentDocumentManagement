using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IStudentRepository _repository;

        public StudentController(IStudentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetPendingStudents")]
        public async Task<IActionResult> GetPendingStudents()
        {
            var (message, students) = await _repository.GetPendingStudentsAsync();

            return Ok(new { message, students });
        }


        [HttpPatch("ApproveRejectStudents")]
        public async Task<IActionResult> UpdateStatus([FromBody] StudentStatusUpdateDto dto)
        {
            int statusId = dto.IsApproved ? 2 : 3;

            var (success, message) = await _repository.UpdateStudentStatusAsync(dto.UserId, statusId);

            if (!success)
                return Ok(new { message });
                //return BadRequest(new { message });


            return Ok(new { message });
        }

        
    }
}