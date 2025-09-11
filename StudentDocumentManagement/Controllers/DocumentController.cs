using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = "Bearer")]
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


        [Authorize(Roles = "Student")]
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


        //[HttpGet("download/{documentId}")]
        //public async Task<IActionResult> DownloadDocument(int documentId)
        //{
        //    // Use repository to get document by ID
        //    var doc = await _documentRepository.GetByIdAsync(documentId);
        //    if (doc == null || !System.IO.File.Exists(doc.FilePath))
        //        return NotFound("File not found.");

        //    var fileBytes = await System.IO.File.ReadAllBytesAsync(doc.FilePath);
        //    var contentType = "application/octet-stream"; // You can use MimeMapping if needed
        //    return File(fileBytes, contentType, doc.FileName);
        //}


        [Authorize(Roles = "Student, Admin")]

        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var result = await _documentRepository.GetDocumentForDownloadAsync(documentId, user);
            if (result == null)
                return StatusCode(403, "You are not allowed to download this document or it does not exist.");

            return File(result.Value.FileBytes, "application/octet-stream", result.Value.FileName);
        }



    }
}
