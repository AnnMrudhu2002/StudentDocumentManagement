using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }
        // API to get summary data for the admin dashboard
        [HttpGet("Summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            // Count all users whose status is Pending (StatusId == 1)
            var pending = await _context.Users
                .Where(u => u.StatusId == 1) // Pending
                .CountAsync();


            // Count all students who have any documents still Pending (StatusId == 1)
            var studentsWithPendingDocs = await _context.Students
        .Where(s => s.Documents.Any(d => d.StatusId == 1))
          .CountAsync();

            // Return the counts as JSON in 200 OK response
            return Ok(new
            {
                PendingStudents = pending,
                StudentsWithPendingDocuments = studentsWithPendingDocs
            });
        }
    }
}
