using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    //[Authorize(Roles = "Admin")]
    public class AdministrativeController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrativeController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // API to create a new role
        [HttpPost("CreateRole")]

        public async Task<IActionResult> CreateRoles(CreateRoleDto model)
        {
            // Validate input
            if (model == null || string.IsNullOrWhiteSpace(model.RoleName))
            {
                return BadRequest(new { message = "Role name is required" });
            }
            // Check if role already exists
            var existingRole = await _roleManager.FindByNameAsync(model.RoleName);
            if (existingRole != null)
            {
               
                return BadRequest(new { message = "Role already exists" });
            }
            // Create new role
            var newRole = new IdentityRole(model.RoleName);
            var creationResult = await _roleManager.CreateAsync(newRole);
            // Return success or error
            if (creationResult.Succeeded)
            {
               
                return Ok(new { message = "Role created successfully" });
            }

            return BadRequest(new { message = "Error creating role" });
        }

        // API to list all roles
        [HttpGet("ListRoles")]
        public async Task<IActionResult> ListRoles()
        {
            // Fetch all roles from Identity
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }
        // API to assign a role to a user
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto request)
        {
            // Find the user by email
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null)
                return NotFound("User not found");
            // Check if role exists
            if (!await _roleManager.RoleExistsAsync(request.RoleName))
                return BadRequest("Role does not exist");
            // Assign role to user
            var result = await _userManager.AddToRoleAsync(user, request.RoleName);
            // Return success or errors
            if (result.Succeeded)
                return Ok(new { message = "Role assigned to user" });


            return BadRequest(result.Errors);
        }
    }
}
