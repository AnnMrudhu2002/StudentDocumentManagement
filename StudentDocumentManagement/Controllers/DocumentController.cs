using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        // get student documents
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


        // download document
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


        // reuplaod document
        [HttpPost("reupload/{documentId}")]
        public async Task<IActionResult> ReUploadDocument(int documentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var existingDoc = await _documentRepository.GetByIdAsync(documentId);
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
            existingDoc.Remarks = null; // clear remarks on reuploading
            existingDoc.UploadedOn = DateTime.UtcNow;

            // Update in repository
            var updated = await _documentRepository.UpdateStatusAsync(existingDoc.DocumentId, existingDoc.StatusId, existingDoc.Remarks);
            if (!updated)
                return BadRequest("Failed to update document.");

            return Ok(new { message = "File re-uploaded successfully" });
        }


        // delete document
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var (success, message) = await _documentRepository.DeleteDocumentAsync(user, documentId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }




    }
}
