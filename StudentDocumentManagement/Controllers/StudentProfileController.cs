using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Student")]
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentProfileController(IStudentProfileRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpPost("SubmitProfile")]
        public async Task<IActionResult> SubmitProfile([FromBody] StudentProfileDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var (success, message, student) = await _repository.SubmitProfileAsync(user, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, studentId = student!.StudentId });
        }
    }
}
