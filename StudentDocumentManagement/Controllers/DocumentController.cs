using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentController(IDocumentRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        [HttpPost("UploadDocument")]
        public async Task<IActionResult> UploadDocument(
     IFormFile file,
     [FromForm] int documentTypeId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            var fileDto = new FileUploadDto
            {
                FileName = file.FileName,
                FileStream = file.OpenReadStream(),
                DocumentTypeId = documentTypeId
            };

            var (success, message, document) = await _repo.UploadDocumentAsync(user, fileDto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, documentId = document!.DocumentId });
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc == null) return NotFound();

            return Ok(doc);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var docs = await _repo.GetStudentDocumentsWithDetailsAsync(studentId);
            return Ok(docs);
        }

        [HttpPut("status/{documentId}")]
        public async Task<IActionResult> UpdateStatus(int documentId, [FromQuery] int statusId, [FromQuery] string? remarks)
        {
            var updated = await _repo.UpdateStatusAsync(documentId, statusId, remarks);
            if (!updated) return NotFound();

            return Ok(new { message = "Document status updated" });
        }




    }
}
