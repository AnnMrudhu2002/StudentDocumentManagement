using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;

        public StudentProfileController(IStudentProfileRepository repository, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _repository = repository;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _repository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound(new { message = "Profile not found" });

            return Ok(student);
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

        [HttpGet("GetEducation")]
        public async Task<IActionResult> GetEducation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _repository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound(new { message = "Student profile not found" });

            var educationList = await _repository.GetEducationByStudentIdAsync(student.StudentId);

            if (educationList == null || educationList.Count == 0)
                return NotFound(new { message = "Education details not found" });

            // Map entity → DTO
            var result = educationList.Select(e => new StudentEducationDto
            {
                EducationLevel = e.EducationLevel,   // ensure column exists
                InstituteName = e.InstituteName,
                PassingYear = e.PassingYear,
                MarksPercentage = e.MarksPercentage
            }).ToList();

            return Ok(result);
        }



        [HttpPost("SubmitEducation")]
        public async Task<IActionResult> SubmitEducation([FromBody] StudentEducationListDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _repository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound(new { message = "Student profile not found" });

            foreach (var edu in dto.EducationDetails)
            {
                await _repository.SubmitEducationAsync(student, edu);
            }

            return Ok(new { message = "Education details saved successfully" });
        }



        [HttpGet("IdProofTypes")]
        public async Task<IActionResult> GetIdProofTypes()
        {
            var list = await _context.IdProofTypes
                                     .Select(x => new { x.IdProofTypeId, x.TypeName })
                                     .ToListAsync();
            return Ok(list);
        }

        [HttpGet("Courses")]
        public async Task<IActionResult> GetCourses()
        {
            var list = await _context.Courses
                                     .Select(x => new { x.CourseId, x.CourseName })
                                     .ToListAsync();
            return Ok(list);
        }
        [HttpGet("States")]
        public async Task<IActionResult> GetStates()
        {
            // Hardcoded list of Indian states
            var states = new List<string> {
        "Andhra Pradesh", "Arunachal Pradesh", "Assam", "Bihar", "Chhattisgarh",
        "Goa", "Gujarat", "Haryana", "Himachal Pradesh", "Jharkhand",
        "Karnataka", "Kerala", "Madhya Pradesh", "Maharashtra", "Manipur",
        "Meghalaya", "Mizoram", "Nagaland", "Odisha", "Punjab",
        "Rajasthan", "Sikkim", "Tamil Nadu", "Telangana", "Tripura",
        "Uttar Pradesh", "Uttarakhand", "West Bengal"
    };

            return Ok(states);
        }

    }
}
