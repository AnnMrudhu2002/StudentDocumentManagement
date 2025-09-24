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

    // get students with pending documents
    [HttpGet("Students-for-approval")]
    public async Task<IActionResult> GetStudentsForApproval()
    {
        var students = await _adminRepository.GetStudentsForApprovalAsync();
        return Ok(students);
    }

    //get documents of particular student
    [HttpGet("Student/{studentId}/Documents")]
    public async Task<IActionResult> GetDocumentsByStudentId(int studentId)
    {
        var docs = await _adminRepository.GetDocumentsByStudentIdAsync(studentId);
        return Ok(docs);
    }

    // approve or reject document
    [HttpPost("Document/{documentId}/Update-status")]
    public async Task<IActionResult> UpdateDocumentStatus(int documentId, [FromQuery] int statusId, [FromQuery] string? remarks)
    {
        var success = await _adminRepository.UpdateDocumentStatusAsync(documentId, statusId, remarks);
        if (!success) return NotFound("Document not found");
        return Ok(new { message = "Status updated successfully" });
    }

    //admin side student-profile and educational details viewing
    [HttpGet("GetProfilePage")]
    public async Task<IActionResult> GetStudentProfile([FromQuery] int studentId)
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
        var educations = await _adminRepository.GetStudentEducationsAsync(studentId);

        // Map to DTO
        var profileDto = new ProfilePageDto
        {
            DOB = student.DOB,
            GenderId = student.GenderId,
            GenderName = student.Gender?.Name,
            PhoneNumber = student.PhoneNumber,
            AlternatePhoneNumber = student.AlternatePhoneNumber,
            Address = student.Address,
            PermanentAddress = student.PermanentAddress,
            IdProofTypeId = student.IdProofTypeId,
            IdProofNumber = student.IdProofNumber,
            City = student.City,
            State = student.State,
            Pincode = student.Pincode,
            District = student.District,
            FullName = user.FullName,
            Email = user.Email,
            CourseName = student.Course?.CourseName,
            Educations = educations
        };

        return Ok(profileDto);
    }

    [HttpGet("GetDocuments")]
    public async Task<IActionResult> GetDocuments(
     [FromQuery] string? branch,
     [FromQuery] string? name,
     [FromQuery] string? registerNo,
     [FromQuery] int? statusId)   
    {
        var query = _context.Documents
         .Include(d => d.Student).ThenInclude(s => s.Course)
         .Include(d => d.Status)
         .AsQueryable();

        bool isFilterApplied = !string.IsNullOrEmpty(branch) || !string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(registerNo);

        if (statusId.HasValue)
        {
            // Filter by the provided status
            query = query.Where(d => d.StatusId == statusId.Value);
        }
        else if (isFilterApplied)
        {
            // If filters like branch/name/registerNo are applied but no statusId, only approved
            query = query.Where(d => d.Status.StatusName == "Approved"); // or StatusId == 1
        }

        // Apply text filters
        if (!string.IsNullOrEmpty(branch))
            query = query.Where(d => d.Student.Course.CourseName.ToLower().Contains(branch.ToLower()));

        if (!string.IsNullOrEmpty(name))
            query = query.Where(d => d.Student.User.FullName.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrEmpty(registerNo))
            query = query.Where(d => d.Student.RegisterNo.ToLower().Contains(registerNo.ToLower()));

        var docs = await query.Select(d => new
        {
            d.DocumentId,
            d.FileName,
            d.FilePath,
            d.StatusId,
            Status = d.Status.StatusName,
            d.UploadedOn,
            StudentName = d.Student.User.FullName,
            RegisterNo = d.Student.RegisterNo,
            Branch = d.Student.Course.CourseName,
            DocumentTypeName = d.DocumentType.TypeName
        }).ToListAsync();

        return Ok(docs);
    }


    [HttpGet("Courses")]
    public async Task<IActionResult> GetCourses()
    {
        var list = await _context.Courses
                                 .Select(x => new { x.CourseId, x.CourseName })
                                 .ToListAsync();
        return Ok(list);
    }


    [HttpGet("Statuses")]
    public async Task<IActionResult> GetStatuses()
    {
        var list = await _context.StatusMasters
                                 .Where(s => new[] { 1, 2, 3 }.Contains(s.StatusId)) 
                                 .Select(s => new
                                 {
                                     s.StatusId,
                                     s.StatusName
                                 })
                                 .ToListAsync();

        return Ok(list);
    }

    [HttpGet("GetFilteredStudents")]
    public async Task<IActionResult> GetFilteredStudents([FromQuery] string branch = "", [FromQuery] string name = "", [FromQuery] string registerNo = "")
    {
        var students = await _adminRepository.GetFilteredStudentsAsync(branch, name, registerNo);
        return Ok(students);
    }

    [HttpGet("GetStudentProfile")]
    public async Task<IActionResult> GetStudentProfileAndEducation([FromQuery] int studentId)
    {
        var profile = await _adminRepository.GetStudentProfileAsync(studentId);
        if (profile == null) return NotFound(new { message = "Profile not found" });
        return Ok(profile);
    }
}
