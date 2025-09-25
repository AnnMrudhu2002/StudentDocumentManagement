using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using StudentDocumentManagement.Controllers;
using System.Security.Claims;

namespace StudentDocManagement.Tests
{
    public class StudentProfileControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IStudentProfileRepository> _studentProfileRepositoryMock;
        private readonly StudentProfileController _controller;
        private readonly AppDbContext _context;

        public StudentProfileControllerTests()
        {
            // Mock UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);



            // Mock repository
            _studentProfileRepositoryMock = new Mock<IStudentProfileRepository>();

            // InMemory DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new AppDbContext(options);

            // Create controller with 3 params
            _controller = new StudentProfileController(
                _studentProfileRepositoryMock.Object,
                _userManagerMock.Object,
                _context
            );

            // Fake user claims for logged-in student
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }


        [Fact]
        public async Task SubmitProfile_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser?)null);

            var dto = new StudentProfileDto();

            // Act
            var result = await _controller.SubmitProfile(dto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not found", unauthorized.Value.GetType().GetProperty("message")?.GetValue(unauthorized.Value));
        }

        [Fact]
        public async Task SubmitProfile_ReturnsBadRequest_WhenRepositoryFails()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-user-id", RegisterNo = "REG123" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _studentProfileRepositoryMock.Setup(r => r.SubmitProfileAsync(user, It.IsAny<StudentProfileDto>()))
                .ReturnsAsync((false, "Error occurred", (Student?)null));

            var dto = new StudentProfileDto();

            // Act
            var result = await _controller.SubmitProfile(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error occurred", badRequest.Value.GetType().GetProperty("message")?.GetValue(badRequest.Value));
        }

        [Fact]
        public async Task SubmitProfile_ReturnsOk_WhenRepositorySucceeds()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-user-id", RegisterNo = "REG123" };
            var student = new Student { StudentId = 1, UserId = user.Id };

            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _studentProfileRepositoryMock.Setup(r => r.SubmitProfileAsync(user, It.IsAny<StudentProfileDto>()))
                .ReturnsAsync((true, "Profile submitted successfully", student));

            var dto = new StudentProfileDto();

            // Act
            var result = await _controller.SubmitProfile(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value!.GetType();
            Assert.Equal("Profile submitted successfully", value.GetProperty("message")?.GetValue(ok.Value));
            Assert.Equal(1, value.GetProperty("studentId")?.GetValue(ok.Value));
        }
    }
}
