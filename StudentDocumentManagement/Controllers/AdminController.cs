using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminRepository _repo;

    public AdminController(IAdminRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("students-for-approval")]
    public async Task<IActionResult> GetStudentsForApproval()
    {
        var students = await _repo.GetStudentsForApprovalAsync();
        return Ok(students);
    }

    [HttpGet("student/{studentId}/documents")]
    public async Task<IActionResult> GetDocumentsByStudentId(int studentId)
    {
        var docs = await _repo.GetDocumentsByStudentIdAsync(studentId);
        return Ok(docs);
    }

    [HttpPost("document/{documentId}/update-status")]
    public async Task<IActionResult> UpdateDocumentStatus(int documentId, [FromQuery] int statusId, [FromQuery] string? remarks)
    {
        var success = await _repo.UpdateDocumentStatusAsync(documentId, statusId, remarks);
        if (!success) return NotFound("Document not found");
        return Ok(new { message = "Status updated successfully" });
    }
}
