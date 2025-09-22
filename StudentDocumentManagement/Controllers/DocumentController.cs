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
        private readonly IDocumentRepository _documentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentProfileRepository _studentProfileRepository;
        private readonly AppDbContext _context;

        public DocumentController(IDocumentRepository documentRepository, UserManager<ApplicationUser> userManager, IStudentProfileRepository studentProfileRepository,AppDbContext context)
        {
            _documentRepository = documentRepository;
            _userManager = userManager;
            _studentProfileRepository = studentProfileRepository;
            _context = context;
        }


        // upload document
        [HttpPost("UploadDocument")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Student")]

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

        // get student documents
        [Authorize(Roles = "Student")]
        [HttpGet("My-documents")]
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


        // download document
        [Authorize(Roles = "Student, Admin")]
        [HttpGet("Download")]
        public async Task<IActionResult> DownloadDocument([FromQuery] int documentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var result = await _documentRepository.GetDocumentForDownloadAsync(documentId, user);
            if (result == null)
                return StatusCode(403, "You are not allowed to download this document or it does not exist.");

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

            var (success, message) = await _documentRepository.DeleteDocumentAsync(user, documentId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [AllowAnonymous]
        [HttpGet("GetAllDocumentType")]
        public async Task<ActionResult<IEnumerable<DocumentType>>> GetDocumentTypes()
        {
            var types = await _context.DocumentTypes.ToListAsync();
            return Ok(types);
        }


    }
}
