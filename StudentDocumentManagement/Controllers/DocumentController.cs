using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IStudentProfileRepository _repository;

        public DocumentController(IDocumentRepository repo, UserManager<ApplicationUser> userManager, IStudentProfileRepository repository)
        {
            _repo = repo;
            _userManager = userManager;
            _repository = repository;
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





        [HttpGet("my-documents")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _repository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound("Student profile not found");

            var docs = await _repo.GetStudentDocumentsWithDetailsAsync(student.StudentId);
            return Ok(docs);
        }


        [HttpPut("status/{documentId}")]
        public async Task<IActionResult> UpdateStatus(int documentId, [FromQuery] int statusId, [FromQuery] string? remarks)
        {
            var updated = await _repo.UpdateStatusAsync(documentId, statusId, remarks);
            if (!updated) return NotFound();

            return Ok(new { message = "Document status updated" });
        }

















        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            // Use repository to get document by ID
            var doc = await _repo.GetByIdAsync(documentId);
            if (doc == null || !System.IO.File.Exists(doc.FilePath))
                return NotFound("File not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(doc.FilePath);
            var contentType = "application/octet-stream"; // You can use MimeMapping if needed
            return File(fileBytes, contentType, doc.FileName);
        }

        [HttpPost("reupload/{documentId}")]
        public async Task<IActionResult> ReUploadDocument(int documentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Get existing document
            var existingDoc = await _repo.GetByIdAsync(documentId);
            if (existingDoc == null)
                return NotFound("Document not found.");

            // Save new file to server
            var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(storagePath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update document fields
            existingDoc.FileName = uniqueFileName;
            existingDoc.FilePath = filePath;
            existingDoc.StatusId = 1; // Reset to Pending
            existingDoc.Remarks = null;
            existingDoc.UploadedOn = DateTime.UtcNow;

            // Update in repository
            var updated = await _repo.UpdateStatusAsync(existingDoc.DocumentId, existingDoc.StatusId, existingDoc.Remarks);
            if (!updated)
                return BadRequest("Failed to update document.");

            return Ok(new { message = "File re-uploaded successfully" });
        }



        [HttpGet("{id}/view")]
        public async Task<IActionResult> ViewDocument(int id)
        {
            var doc = await _repo.GetByIdAsync(id);
            if (doc == null || string.IsNullOrEmpty(doc.FilePath) || !System.IO.File.Exists(doc.FilePath))
                return NotFound("Document not found");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(doc.FilePath);
            var ext = Path.GetExtension(doc.FileName).ToLowerInvariant();

            var contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" or ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" or ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            // returning without filename → lets browser show inline
            return File(fileBytes, contentType);
        }


    }
}
