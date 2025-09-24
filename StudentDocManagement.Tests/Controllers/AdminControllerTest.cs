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
        private readonly Mock<IAdminRepository> _mockRepo;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly AppDbContext _context;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockRepo = new Mock<IAdminRepository>();

            var storeMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                storeMock.Object, null, null, null, null, null, null, null, null);

            // In-memory DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            _controller = new AdminController(_mockRepo.Object, _mockUserManager.Object, _context);
        }

        #region GetStudentsForApproval
        [Fact]
        public async Task GetStudentsForApproval_ReturnsOkWithList()
        {
         

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

            var result = await _controller.GetStudentsForApproval();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(students, ok.Value);
        }
        #endregion

        #region GetDocumentsByStudentId
        [Fact]
        public async Task GetDocumentsByStudentId_ReturnsOkWithDocs()
        {
            int studentId = 1;

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

            var result = await _controller.GetDocumentsByStudentId(studentId);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(docs, ok.Value);
        }
        #endregion

        #region UpdateDocumentStatus
        [Fact]
        public async Task UpdateDocumentStatus_Success_ReturnsOk()
        {
            _mockRepo.Setup(x => x.UpdateDocumentStatusAsync(1, 2, "Approved"))
                     .ReturnsAsync(true);

            var result = await _controller.UpdateDocumentStatus(1, 2, "Approved");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Status updated successfully", ok.Value.ToString());
        }

        [Fact]
        public async Task UpdateDocumentStatus_NotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(x => x.UpdateDocumentStatusAsync(1, 2, "Approved"))
                     .ReturnsAsync(false);

            var result = await _controller.UpdateDocumentStatus(1, 2, "Approved");

            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion

        #region GetStudentProfile
        [Fact]
        public async Task GetStudentProfile_ExistingStudent_ReturnsOk()
        {
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

            _mockRepo.Setup(x => x.GetStudentByIdAsync(1)).ReturnsAsync(student);
            _mockRepo.Setup(x => x.GetStudentEducationsAsync(1)).ReturnsAsync(educations);
            _mockUserManager.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync(student.User);

            var result = await _controller.GetStudentProfile(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<ProfilePageDto>(ok.Value);

            Assert.Equal("John Doe", profile.FullName);
            Assert.Equal(educations, profile.Educations);
        }

        [Fact]
        public async Task GetStudentProfile_StudentNotFound_ReturnsNotFound()
        {
            _mockRepo.Setup(x => x.GetStudentByIdAsync(1)).ReturnsAsync((Student?)null);

            var result = await _controller.GetStudentProfile(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetStudentProfile_UserNotFound_ReturnsNotFound()
        {
            var student = new Student { StudentId = 1, UserId = "user1" };
            _mockRepo.Setup(x => x.GetStudentByIdAsync(1)).ReturnsAsync(student);
            _mockUserManager.Setup(x => x.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser?)null);

            var result = await _controller.GetStudentProfile(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion
    }
}
