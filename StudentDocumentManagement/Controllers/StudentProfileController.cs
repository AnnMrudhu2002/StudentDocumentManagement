using Microsoft.AspNetCore.Authorization;
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
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public StudentProfileController(IStudentProfileRepository studentProfileRepository, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _studentProfileRepository = studentProfileRepository;
            _userManager = userManager;
            _context = context;
        }


        // get student profile details
        [Authorize(Roles = "Student")]
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(new { message = "User not found" });

            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student is null)
                return Ok(null);



            var profileDto = new StudentProfileDto
            {
                DOB = student.DOB,
                GenderId = student.GenderId,
                PhoneNumber = student.PhoneNumber,
                AlternatePhoneNumber = student.AlternatePhoneNumber,
                Address = student.Address,
                PermanentAddress = student.PermanentAddress,
                City = student.City,
                State = student.State,
                Pincode = student.Pincode,
                District = student.District,
                IdProofTypeId = student.IdProofTypeId,
                IdProofNumber = student.IdProofNumber,
                CourseId = student.CourseId
            };

            return Ok(profileDto);
        }




        [Authorize(Roles = "Student")]
        [HttpGet("GetProfilePage")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized(new { message = "User not found" });

            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student is null)
                return NotFound(new { message = "Profile not found" });
            //var prof = student.Adapt<StudentProfileDto>();

            var profileDto = new ProfilePageDto
            {
                DOB = student.DOB,
                GenderId = student.GenderId,
                PhoneNumber = student.PhoneNumber,
                AlternatePhoneNumber = student.AlternatePhoneNumber,
                Address = student.Address,
                PermanentAddress = student.PermanentAddress,
                IdProofTypeId = student.IdProofTypeId,
                IdProofNumber = student.IdProofNumber,
                FullName = user.FullName,
                Email = user.Email,
                CourseName = student.Course?.CourseName

            };

            return Ok(profileDto);
        }

        // allow all authenticated users

        // submit student profile details
        [Authorize(Roles = "Student")]
        [HttpPost("SubmitProfile")]
        public async Task<IActionResult> SubmitProfile([FromBody] StudentProfileDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var (success, message, student) = await _studentProfileRepository.SubmitProfileAsync(user, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, studentId = student!.StudentId });
        }


        // get education details
        [Authorize(Roles = "Student")]
        [HttpGet("GetEducation")]
        public async Task<IActionResult> GetEducation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound(new { message = "Student profile not found" });

            var educationList = await _studentProfileRepository.GetEducationByStudentIdAsync(student.StudentId);

            if (educationList == null || educationList.Count == 0)
                return Ok(new List<StudentEducationDto>());

            var result = educationList.Select(e => new StudentEducationDto
            {
                EducationLevel = e.EducationLevel,   
                InstituteName = e.InstituteName,
                PassingYear = e.PassingYear,
                MarksPercentage = e.MarksPercentage
            }).ToList();

            return Ok(result);
        }


        // submit educational details
        [Authorize(Roles = "Student")]
        [HttpPost("SubmitEducation")]
        public async Task<IActionResult> SubmitEducation([FromBody] StudentEducationListDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound(new { message = "Student profile not found" });

            foreach (var edu in dto.EducationDetails)
            {
                await _studentProfileRepository.SubmitEducationAsync(student, edu);
            }

            return Ok(new { message = "Education details saved successfully" });
        }


        // get all id proof types
        [Authorize(Roles = "Student")]
        [HttpGet("IdProofTypes")]
        public async Task<IActionResult> GetIdProofTypes()
        {
            var list = await _context.IdProofTypes
                                     .Select(x => new { x.IdProofTypeId, x.TypeName })
                                     .ToListAsync();
            return Ok(list);
        }

        // get all courses
        [Authorize(Roles = "Student, Admin")]
        [HttpGet("Courses")]
        public async Task<IActionResult> GetCourses()
        {
            var list = await _context.Courses
                                     .Select(x => new { x.CourseId, x.CourseName })
                                     .ToListAsync();
            return Ok(list);
        }


        // get all genders
        [Authorize(Roles = "Student")]
        [HttpGet("Genders")]
        public async Task<IActionResult> GetGenders()
        {
            var list = await _context.Genders
                                     .Select(x => new { x.GenderId, x.Name })
                                     .ToListAsync();
            return Ok(list);
        }

        // get all states
        [Authorize(Roles = "Student")]
        [HttpGet("GetAllState")]
        public async Task<IActionResult> GetStates()
        {
            var states = await _studentProfileRepository.GetAllStatesAsync();
            return Ok(states);
        }

        // get district by state
        [Authorize(Roles = "Student")]
        [HttpGet("GetDistricts")]
        public async Task<IActionResult> GetDistricts([FromQuery] int stateId)
        {
            var districts = await _studentProfileRepository.GetDistrictsByStateIdAsync(stateId);
            if (districts == null || !districts.Any())
                return NotFound("No districts found for this state.");

            return Ok(districts);
        }

        // get pincode by district
        [Authorize(Roles = "Student")]
        [HttpGet("Pincodes")]
        public async Task<IActionResult> GetPincodes([FromQuery] int districtId)
        {
            var pincodes = await _studentProfileRepository.GetPincodesByDistrictIdAsync(districtId);
            return Ok(pincodes);
        }

        // get post office by pincode
        [Authorize(Roles = "Student")]
        [HttpGet("Postoffices")]
        public async Task<IActionResult> GetPostOffices([FromQuery] int pincodeId)
        {
            var offices = await _studentProfileRepository.GetPostOfficesByPincodeIdAsync(pincodeId);
            return Ok(offices);
        }

        // acknowledgement for profile submission
        [Authorize(Roles = "Student")]
        [HttpPost("Acknowledge")]
        public async Task<IActionResult> Acknowledge()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var (success, message) = await _studentProfileRepository.AcknowledgeAsync(user.Id);
            if (!success) return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
