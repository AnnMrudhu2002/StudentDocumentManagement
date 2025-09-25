using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using StudentDocumentManagement.Controllers;
using System.Security.Claims;
using Xunit;

namespace StudentDocManagement.Tests
{
    public class DocumentControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IDocumentRepository> _documentRepositoryMock;
        private readonly Mock<IStudentProfileRepository> _studentProfileRepositoryMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly AppDbContext _context;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            // Setup in-memory EF Core DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new AppDbContext(options);

            // Mock dependencies
            _documentRepositoryMock = new Mock<IDocumentRepository>();
            _studentProfileRepositoryMock = new Mock<IStudentProfileRepository>();
            _envMock = new Mock<IWebHostEnvironment>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Create controller with ALL dependencies
            _controller = new DocumentController(
                _documentRepositoryMock.Object,
                _userManagerMock.Object,
                _studentProfileRepositoryMock.Object,
                _context,
                _envMock.Object
            );

            // Fake logged-in user claims
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
        public async Task DeleteDocument_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser?)null);

            // Act
            var result = await _controller.DeleteDocument(1);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not found", unauthorized.Value!.GetType().GetProperty("message")?.GetValue(unauthorized.Value));
        }

        [Fact]
        public async Task DeleteDocument_ReturnsBadRequest_WhenRepositoryFails()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-user-id" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _documentRepositoryMock.Setup(r => r.DeleteDocumentAsync(user, 1))
                .ReturnsAsync((false, "Document not found"));

            // Act
            var result = await _controller.DeleteDocument(1);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Document not found", badRequest.Value!.GetType().GetProperty("message")?.GetValue(badRequest.Value));
        }

        [Fact]
        public async Task DeleteDocument_ReturnsOk_WhenRepositorySucceeds()
        {
            // Arrange
            var user = new ApplicationUser { Id = "test-user-id" };
            _userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _documentRepositoryMock.Setup(r => r.DeleteDocumentAsync(user, 1))
                .ReturnsAsync((true, "Document deleted successfully"));

            // Act
            var result = await _controller.DeleteDocument(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Document deleted successfully", ok.Value!.GetType().GetProperty("message")?.GetValue(ok.Value));
        }
    }
}
