using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminDashboard/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            // Students are AspNetUsers with Role = "Student"
            // and their StatusId comes from ApplicationUser

            var pending = await _context.Users
                .Where(u => u.StatusId == 1) // Pending
                .CountAsync();

            var approved = await _context.Users
                .Where(u => u.StatusId == 2) // Approved
                .CountAsync();

            return Ok(new
            {
                PendingStudents = pending,
                ApprovedStudents = approved
            });
        }
    }
}
