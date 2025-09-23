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

        // get admin dashboard
        [HttpGet("Summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {

            var pending = await _context.Users
                .Where(u => u.StatusId == 1) // Pending
                .CountAsync();



            var studentsWithPendingDocs = await _context.Students
        .Where(s => s.Documents.Any(d => d.StatusId == 1))
          .CountAsync();

            return Ok(new
            {
                PendingStudents = pending,
                StudentsWithPendingDocuments = studentsWithPendingDocs
            });
        }
    }
}
