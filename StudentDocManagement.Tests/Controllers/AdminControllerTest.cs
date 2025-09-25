using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using Xunit;

namespace StudentDocManagement.Tests
{
    public class AdminControllerTests
    {
        // Mocked dependencies for the controller
        private readonly Mock<IAdminRepository> _mockRepo;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly AppDbContext _context;
        private readonly AdminController _controller;

        // Constructor sets up in-memory database, mocks, and controller
        public AdminControllerTests()
        {
            _mockRepo = new Mock<IAdminRepository>();

            // Mock UserManager (requires IUserStore)
            var storeMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                storeMock.Object, null, null, null, null, null, null, null, null);

            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            // Initialize controller with mocks
            _controller = new AdminController(_mockRepo.Object, _mockUserManager.Object, _context);
        }

        #region GetStudentsForApproval
        [Fact]
        public async Task GetStudentsForApproval_ReturnsOkWithList()
        {
            // Arrange: mock repository to return a sample student
            var students = new List<StudentWithDocsDto>
            {
                new StudentWithDocsDto
                {
                    StudentId = 1,
                    FullName = "John Doe",
                    RegisterNo = "REG123"
                }
            };
            _mockRepo.Setup(x => x.GetStudentsForApprovalAsync()).ReturnsAsync(students);

            // Act: call controller method
            var result = await _controller.GetStudentsForApproval();

            // Assert: check result is OkObjectResult and value matches
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(students, ok.Value);
        }
        #endregion

        #region GetDocumentsByStudentId
        [Fact]
        public async Task GetDocumentsByStudentId_ReturnsOkWithDocs()
        {
            int studentId = 1;

            // Arrange: mock repository to return sample documents
            var docs = new List<StudentDocumentDto>
            {
                new StudentDocumentDto
                {
                    DocumentId = 1,
                    DocumentTypeName = "ID Proof",
                    StatusName = "Pending",
                    Remarks = "Need verification"
                }
            };
            _mockRepo.Setup(x => x.GetDocumentsByStudentIdAsync(studentId))
                     .ReturnsAsync(docs);

            // Act
            var result = await _controller.GetDocumentsByStudentId(studentId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(docs, ok.Value);
        }
        #endregion

        #region UpdateDocumentStatus
        [Fact]
        public async Task UpdateDocumentStatus_Success_ReturnsOk()
        {
            // Arrange: mock UpdateDocumentStatusAsync to return true
            _mockRepo.Setup(x => x.UpdateDocumentStatusAsync(1, 2, "Approved"))
                     .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateDocumentStatus(1, 2, "Approved");

            // Assert: result is Ok and contains success message
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Status updated successfully", ok.Value.ToString());
        }

        [Fact]
        public async Task UpdateDocumentStatus_NotFound_ReturnsNotFound()
        {
            // Arrange: mock UpdateDocumentStatusAsync to return false
            _mockRepo.Setup(x => x.UpdateDocumentStatusAsync(1, 2, "Approved"))
                     .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateDocumentStatus(1, 2, "Approved");

            // Assert: result is NotFoundObjectResult
            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion

        #region GetStudentProfile
        [Fact]
        public async Task GetStudentProfile_ExistingStudent_ReturnsOk()
        {
            // Arrange: create sample student and education data
            var student = new Student
            {
                StudentId = 1,
                UserId = "user1",
                GenderId = 1,
                User = new ApplicationUser
                {
                    Id = "user1",
                    FullName = "John Doe",
                    Email = "john@test.com"
                }
            };

            var educations = new List<StudentEducationDto>
            {
                new StudentEducationDto
                {
                    EducationLevel = "Bachelor",
                    InstituteName = "XYZ University",
                    PassingYear = 2020,
                    MarksPercentage = 75
                }
            };

            // Mock repository and UserManager calls
            _mockRepo.Setup(x => x.GetStudentByIdAsync(1)).ReturnsAsync(student);
            _mockRepo.Setup(x => x.GetStudentEducationsAsync(1)).ReturnsAsync(educations);
            _mockUserManager.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync(student.User);

            // Act
            var result = await _controller.GetStudentProfile(1);

            // Assert: check OkObjectResult and profile data
            var ok = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<ProfilePageDto>(ok.Value);

            Assert.Equal("John Doe", profile.FullName);
            Assert.Equal(educations, profile.Educations);
        }

        [Fact]
        public async Task GetStudentProfile_StudentNotFound_ReturnsNotFound()
        {
            // Arrange: mock repository returns null
            _mockRepo.Setup(x => x.GetStudentByIdAsync(1)).ReturnsAsync((Student?)null);

            // Act
            var result = await _controller.GetStudentProfile(1);

            // Assert: NotFoundObjectResult returned
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetStudentProfile_UserNotFound_ReturnsNotFound()
        {
            // Arrange: student exists but UserManager returns null
            var student = new Student { StudentId = 1, UserId = "user1" };
            _mockRepo.Setup(x => x.GetStudentByIdAsync(1)).ReturnsAsync(student);
            _mockUserManager.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.GetStudentProfile(1);

            // Assert: NotFoundObjectResult returned
            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion
    }
}
