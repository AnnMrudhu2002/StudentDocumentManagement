using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminRepository _adminRepository;

    public AdminController(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
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

}
