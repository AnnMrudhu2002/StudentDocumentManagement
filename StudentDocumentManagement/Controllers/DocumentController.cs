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
        private readonly IDocumentRepository _documentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentProfileRepository _studentProfileRepository;

        public DocumentController(IDocumentRepository documentRepository, UserManager<ApplicationUser> userManager, IStudentProfileRepository studentProfileRepository)
        {
            _documentRepository = documentRepository;
            _userManager = userManager;
            _studentProfileRepository = studentProfileRepository;
        }

        [HttpPost("UploadDocument")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] int documentTypeId)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var fileDto = new FileUploadDto
            {
                FileName = file.FileName,
                FileStream = file.OpenReadStream(),
                DocumentTypeId = documentTypeId,
                ContentType = file.ContentType,
                FileSize = file.Length
            };

            var (success, message, document) = await _documentRepository.UploadDocumentAsync(user, fileDto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, documentId = document!.DocumentId });
        }

        [HttpGet("GetStudentDocuments")]
        public async Task<IActionResult> GetStudentDocuments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            var documents = await _documentRepository.GetStudentDocumentDetails(student.StudentId);
            return Ok(documents);
        }


        [HttpGet("my-documents")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound("Student profile not found");

            var docs = await _documentRepository.GetStudentDocumentsWithDetailsAsync(student.StudentId);
            return Ok(docs);
        }


        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            // Use repository to get document by ID
            var doc = await _documentRepository.GetByIdAsync(documentId);
            if (doc == null || !System.IO.File.Exists(doc.FilePath))
                return NotFound("File not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(doc.FilePath);
            var contentType = "application/octet-stream"; // You can use MimeMapping if needed
            return File(fileBytes, contentType, doc.FileName);
        }

     
    }
}
