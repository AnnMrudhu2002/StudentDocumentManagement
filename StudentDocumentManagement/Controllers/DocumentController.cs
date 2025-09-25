using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository; // Repository for handling document operations
        private readonly UserManager<ApplicationUser> _userManager; // Identity user manager
        private readonly IStudentProfileRepository _studentProfileRepository; // Repository for student profile
        private readonly AppDbContext _context; // EF Core database context
        private readonly IWebHostEnvironment _env; // For file storage path, environment info

        public DocumentController(IDocumentRepository documentRepository, UserManager<ApplicationUser> userManager,
            IStudentProfileRepository studentProfileRepository,AppDbContext context, IWebHostEnvironment env)
        {
            _documentRepository = documentRepository;
            _userManager = userManager;
            _studentProfileRepository = studentProfileRepository;
            _context = context;
            _env = env;
        }

        // Upload a new document (only students can upload)
        [HttpPost("UploadDocument")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Student")]

        public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] int documentTypeId)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });
            // Prepare DTO for repository
            var fileDto = new FileUploadDto
            {
                FileName = file.FileName,
                FileStream = file.OpenReadStream(),
                DocumentTypeId = documentTypeId,
                ContentType = file.ContentType,
                FileSize = file.Length
            };
            // Call repository to save file
            var (success, message, document) = await _documentRepository.UploadDocumentAsync(user, fileDto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message, documentId = document!.DocumentId });
        }



        //[Authorize(Roles = "Student")]
        //[HttpGet("GetStudentDocuments")]
        //public async Task<IActionResult> GetStudentDocuments()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //        return Unauthorized(new { message = "User not found" });

        //    var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
        //    if (student == null)
        //        return NotFound(new { message = "Student not found" });

        //    var documents = await _documentRepository.GetStudentDocumentsWithDetailsAsync(student.StudentId);
        //    return Ok(documents);
        //}



        // Get documents of the logged-in student
        [Authorize(Roles = "Student")]
        [HttpGet("My-documents")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });
            // Find student profile for the logged-in user
            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound("Student profile not found");
            // Fetch all documents of the student
            var docs = await _documentRepository.GetStudentDocumentsWithDetailsAsync(student.StudentId);
            return Ok(docs);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("GetStudentDocuments")]
        public async Task<IActionResult> GetStudentDocuments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var student = await _studentProfileRepository.GetStudentByUserIdAsync(user.Id);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            var documents = await _documentRepository.GetStudentDocumentsWithDetailsAsync(student.StudentId);
            return Ok(documents);
        }


        // Download a document (Students or Admin can download)
        [Authorize(Roles = "Student, Admin")]
        [HttpGet("Download")]
        public async Task<IActionResult> DownloadDocument([FromQuery] int documentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });
            // Get document bytes and name from repository
            var result = await _documentRepository.GetDocumentForDownloadAsync(documentId, user);
            if (result == null)
                return StatusCode(403, "You are not allowed to download this document or it does not exist.");
            // Return file as download
            return File(result.Value.FileBytes, "application/octet-stream", result.Value.FileName);
        }



        // reuplaod document
        [HttpPost("Reupload")]
        public async Task<IActionResult> ReUploadDocument([FromQuery] int documentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var existingDoc = await _documentRepository.GetByIdAsync(documentId);
            if (existingDoc == null)
                return NotFound("Document not found.");

            var success = await _documentRepository.ReUploadDocumentAsync(existingDoc, file);
            if (!success)
                return BadRequest("Failed to update document.");

            return Ok(new { message = "File re-uploaded successfully" });
        }



        // delete document
        [HttpDelete("DeleteDocument")]
        public async Task<IActionResult> DeleteDocument([FromQuery] int documentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });
            // Call repository to delete document
            var (success, message) = await _documentRepository.DeleteDocumentAsync(user, documentId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        // Get all available document types (no authentication required)
        [AllowAnonymous]
        [HttpGet("GetAllDocumentType")]
        public async Task<ActionResult<IEnumerable<DocumentType>>> GetDocumentTypes()
        {
            var types = await _context.DocumentTypes.ToListAsync();
            return Ok(types);
        }

        // View a document inline (browser opens PDF or image)
        [Authorize(Roles = "Student, Admin")]
        [HttpGet("view/{documentId}")]
        public async Task<IActionResult> ViewDocument(int documentId)
        {
            // Get current user (for authorization)
            var user = await _userManager.GetUserAsync(User);

            var result = await _documentRepository.GetDocumentForDownloadAsync(documentId, user);
            if (result == null)
                return NotFound("Document not found or access denied");

            var (fileBytes, fileName) = result.Value;

            // Determine content type
            var contentType = GetContentType(fileName);

            // Stream file inline (browser opens PDF/image)
            return File(fileBytes, contentType);
        }
        // Helper method to determine MIME type based on file extension
        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }



    }
}
