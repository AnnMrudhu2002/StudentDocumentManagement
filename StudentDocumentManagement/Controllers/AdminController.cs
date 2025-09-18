using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminRepository _adminRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public AdminController(IAdminRepository adminRepository, UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _adminRepository = adminRepository;
        _userManager = userManager;
        _context = context;
    }


    [HttpGet("students-for-approval")]
    public async Task<IActionResult> GetStudentsForApproval()
    {
        var students = await _adminRepository.GetStudentsForApprovalAsync();
        return Ok(students);
    }


    [HttpGet("student/{studentId}/documents")]
    public async Task<IActionResult> GetDocumentsByStudentId(int studentId)
    {
        var docs = await _adminRepository.GetDocumentsByStudentIdAsync(studentId);
        return Ok(docs);
    }


    [HttpPost("document/{documentId}/update-status")]
    public async Task<IActionResult> UpdateDocumentStatus(int documentId, [FromQuery] int statusId, [FromQuery] string? remarks)
    {
        var success = await _adminRepository.UpdateDocumentStatusAsync(documentId, statusId, remarks);
        if (!success) return NotFound("Document not found");
        return Ok(new { message = "Status updated successfully" });
    }
    [HttpGet("GetProfilePage/{studentId}")]
    public async Task<IActionResult> GetStudentProfile(int studentId)
    {
        // Fetch student by ID
        var student = await _adminRepository.GetStudentByIdAsync(studentId);
        if (student == null)
            return NotFound(new { message = "Profile not found" });

        // Fetch corresponding user info
        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user == null)
            return NotFound(new { message = "User not found" });
        // fetch student educations
        var educations = await _context.StudentEducations
            .Where(e => e.StudentId == studentId)
            .Select(e => new StudentEducationDto
            {
                EducationLevel = e.EducationLevel,
                InstituteName = e.InstituteName,
                PassingYear = e.PassingYear,
                MarksPercentage = e.MarksPercentage
            })
            .ToListAsync();

        // Map to DTO
        var profileDto = new ProfilePageDto
        {

            DOB = student.DOB,
            GenderId = student.GenderId,
            PhoneNumber = student.PhoneNumber,
            AlternatePhoneNumber = student.AlternatePhoneNumber,
            Address = student.Address,
            PermanentAddress = student.PermanentAddress,
            City = student.City,
            District = student.District,
            State = student.State,
            Pincode = student.Pincode,
            IdProofTypeId = student.IdProofTypeId,
            IdProofNumber = student.IdProofNumber,
            FullName = user.FullName,
            Email = user.Email,
            CourseName = student.Course?.CourseName,
            Educations = educations
        };

        return Ok(profileDto);
    }
}
