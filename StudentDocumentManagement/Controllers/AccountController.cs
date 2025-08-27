using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Repository;

namespace StudentDocumentManagement.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ITokenRepository _tokenRepository;


        public AuthenticationController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _tokenRepository = tokenRepository;

        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] StudentDocManagement.Entity.Dto.RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if email or register number already exists
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(new { message = "Email already exists" });

            if (_userManager.Users.Any(u => u.RegisterNo == request.RegisterNo))
                return BadRequest(new { message = "Register number already exists" });

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                RegisterNo = request.RegisterNo,
                CreatedOn = DateTime.UtcNow,
                StatusId = 1 // Pending
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "Student");
            return Ok(new { message = "Registration successful", userId = user.Id });
        }



        [HttpPost("AdminRegister")]
        public async Task<IActionResult> AdminRegister([FromBody] AdminRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(new { message = "Email already exists" });

            var user = new ApplicationUser
            {
                UserName = request.FullName,  
                Email = request.Email,
                FullName = request.FullName,
                CreatedOn = DateTime.UtcNow,
                StatusId = 2 // Active
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Admin");
            return Ok(new { message = "Admin Registration successful", userId = user.Id });
        }





        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] StudentDocManagement.Entity.Dto.LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Please fill all details carefully" });

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required" });

            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Validate password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Invalid email or password" });

            // Check account status (optional)
            if (user.StatusId == 1) // Pending
                return Unauthorized(new { message = "Account is pending approval" });

            if (user.StatusId == 3) // Rejected
                return Unauthorized(new { message = "Your account has been rejected. Please contact admin." });
            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Pass roles to token generation if needed
            var token = await _tokenRepository.GetToken(user, roles); // Update your GetToken method

            var userInfo = new UserInfoResultDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = token,
                Roles = roles.ToList(),
                RegisterNo = user.RegisterNo,
            };

            return Ok(userInfo);

        }

    }
}
