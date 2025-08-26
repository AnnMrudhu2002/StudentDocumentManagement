using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrativeController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrativeController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        

        [HttpPost("CreateRole")]

        public async Task<IActionResult> CreateRoles(CreateRoleViewModel model)
        {

            if (model == null || string.IsNullOrWhiteSpace(model.RoleName))
            {
                return BadRequest(new { message = "Role name is required" });
            }

            var existingRole = await _roleManager.FindByNameAsync(model.RoleName);
            if (existingRole != null)
            {
               
                return BadRequest(new { message = "Role already exists" });
            }

            var newRole = new IdentityRole(model.RoleName);
            var creationResult = await _roleManager.CreateAsync(newRole);

            if (creationResult.Succeeded)
            {
               
                return Ok(new { message = "Role created successfully" });
            }

            return BadRequest(new { message = "Error creating role" });
        }


        [HttpGet("ListRoles")]
        public async Task<IActionResult> ListRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null)
                return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(request.RoleName))
                return BadRequest("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);

            if (result.Succeeded)
                return Ok(new { message = "Role assigned to user" });


            return BadRequest(result.Errors);
        }
    }
}
