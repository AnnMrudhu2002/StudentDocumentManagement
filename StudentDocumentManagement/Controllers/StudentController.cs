using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentDocManagement.Services.Repository;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var (success, message, students) = await _repository.GetPendingStudentsAsync();

            if (!success)
                return NotFound(new { message });

            return Ok(new { message, students });
        }
    }
}
