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

            // Find user by email
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                if (existingUser.StatusId == 3) // Rejected
                {
                    // Update existing rejected user
                    existingUser.FullName = request.FullName;
                    existingUser.RegisterNo = request.RegisterNo;
                    existingUser.CreatedOn = DateTime.UtcNow;
                    existingUser.StatusId = 1; // Pending

                    // Update password
                    var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                    var passwordResult = await _userManager.ResetPasswordAsync(existingUser, token, request.Password);

                    if (!passwordResult.Succeeded)
                    {
                        return BadRequest(passwordResult.Errors);
                    }

                    await _userManager.UpdateAsync(existingUser);

                    // Ensure role is Student
                    if (!await _userManager.IsInRoleAsync(existingUser, "Student"))
                    {
                        await _userManager.AddToRoleAsync(existingUser, "Student");
                    }

                    return Ok(new { message = "Registration successful", userId = existingUser.Id });
                }
                else // Pending or Approved
                {
                    return BadRequest(new { message = "Email already exists" });
                }
            }

            // Check register number for other users (ignore rejected)
            var registerNoUser = _userManager.Users.FirstOrDefault(u => u.RegisterNo == request.RegisterNo && u.StatusId != 3);
            if (registerNoUser != null)
                return BadRequest(new { message = "Register number already exists" });

            // Create new user
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
                StatusId = 2 
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

            // Check account status 
            if (user.StatusId == 1) // Pending
                return Unauthorized(new { message = "Account is pending approval" });

            if (user.StatusId == 3) // Rejected
                return Unauthorized(new { message = "Your account has been rejected. Please contact admin." });
            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // Pass roles to token generation if needed
            var token = await _tokenRepository.GetToken(user, roles); 

            var userInfo = new UserInfoResultDto
            {
                Token = token,
            
            };

            return Ok(userInfo);

        }

    }
}
