using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using StudentDocManagement.Services.Repository;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentProfileRepository _studentProfileRepository;

        public StudentDashboardController(IDashboardRepository dashboardRepository,
                                   UserManager<ApplicationUser> userManager,
                                   IStudentProfileRepository studentProfileRepository)
                                   
        {
            _dashboardRepository = dashboardRepository;
            _userManager = userManager;
            _studentProfileRepository = studentProfileRepository;
        }


        [HttpGet("ProfileCompletion")]
        public async Task<IActionResult> GetProfileCompletion()
        {
            // Get logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            // Get student profile
            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
            {
              
                return Ok(new { CompletionPercentage = 0 });
            }
           

            // Get profile completion
            var completion = await _dashboardRepository.GetProfileCompletionAsync(student);

            return Ok(new {CompletionPercentage = completion });
        }

    }
}
