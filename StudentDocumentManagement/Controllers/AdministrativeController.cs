using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;

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
        [HttpGet("ListRoles")]
        public async Task<IActionResult> ListRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpPost("CreateRole")]

        public async Task<IActionResult> CreateRoles(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new()
                {
                    Name = model.RoleName,
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListRoles));
                }
            }
            return Ok();
        }
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
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
