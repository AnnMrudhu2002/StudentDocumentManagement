using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using StudentDocManagement.Services.Repository;

namespace StudentDocManagement.Tests
{
    public class DocumentRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IStudentProfileRepository> _studentRepoMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly DocumentRepository _repository;
        public DocumentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_DocRepo")
                .Options;
            _context = new AppDbContext(options);

            _studentRepoMock = new Mock<IStudentProfileRepository>();

            //  Add mock for UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            // Pass UserManager into repository
            _repository = new DocumentRepository(
                _context,
                _studentRepoMock.Object,
                _userManagerMock.Object
            );
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsFalse_WhenStudentNotFound()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1" };
            _studentRepoMock.Setup(r => r.GetStudentByUserIdAsync("u1"))
                            .ReturnsAsync((Student?)null);

            // Act
            var result = await _repository.DeleteDocumentAsync(user, 1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Student profile not found", result.Message);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsFalse_WhenDocumentNotFound()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1" };
            _studentRepoMock.Setup(r => r.GetStudentByUserIdAsync("u1"))
                            .ReturnsAsync(new Student { StudentId = 1, UserId = "u1" });

            // Act
            var result = await _repository.DeleteDocumentAsync(user, 999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Document not found ", result.Message);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsFalse_WhenDocumentApproved()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1" };
            _studentRepoMock.Setup(r => r.GetStudentByUserIdAsync("u1"))
                            .ReturnsAsync(new Student { StudentId = 1, UserId = "u1" });

            var doc = new Document
            {
                DocumentId = 1,
                StudentId = 1,
                StatusId = 2, // Approved
                FilePath = "dummy.pdf"
            };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteDocumentAsync(user, 1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Approved documents cannot be deleted", result.Message);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsFalse_WhenDocumentRejected()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1" };
            _studentRepoMock.Setup(r => r.GetStudentByUserIdAsync("u1"))
                            .ReturnsAsync(new Student { StudentId = 1, UserId = "u1" });

            var doc = new Document
            {
                DocumentId = 2,
                StudentId = 1,
                StatusId = 3, // Rejected
                FilePath = "dummy.pdf"
            };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteDocumentAsync(user, 2);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Rejected documents cannot be deleted", result.Message);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ReturnsTrue_WhenDocumentDeleted()
        {
            // Arrange
            var user = new ApplicationUser { Id = "u1" };
            _studentRepoMock.Setup(r => r.GetStudentByUserIdAsync("u1"))
                            .ReturnsAsync(new Student { StudentId = 1, UserId = "u1" });

            var doc = new Document
            {
                DocumentId = 3,
                StudentId = 1,
                StatusId = 1, // Pending
                FilePath = "dummy.pdf"
            };
            _context.Documents.Add(doc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteDocumentAsync(user, 3);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Document deleted successfully", result.Message);
            Assert.Null(await _context.Documents.FindAsync(3));
        }
    }
}
