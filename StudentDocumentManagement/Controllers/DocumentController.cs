using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocumentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _repo;

        public DocumentController(IDocumentRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("upload/{studentId}")]
        public async Task<IActionResult> UploadDocument(
            int studentId,
            IFormFile file,
            [FromForm] int documentTypeId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Save file outside wwwroot
            var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(storagePath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                StudentId = studentId,
                DocumentTypeId = documentTypeId,
                FileName = uniqueFileName,
                FilePath = filePath,
                StatusId = 1, // Pending
                UploadedOn = DateTime.UtcNow
            };

            var savedDoc = await _repo.UploadAsync(document);
            return Ok(new { message = "Document uploaded successfully", documentId = savedDoc.DocumentId });
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
            var docs = await _repo.GetByStudentIdAsync(studentId);
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
