using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Dto;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Repository;

namespace StudentDocManagement.Tests
{
    public class StudentProfileRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly StudentProfileRepository _repository;

        public StudentProfileRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // fresh DB per test
                .Options;

            _context = new AppDbContext(options);
            _repository = new StudentProfileRepository(_context);
        }

        [Fact]
        public async Task SubmitProfileAsync_CreatesNewProfile_WhenNoneExists()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user1", RegisterNo = "REG123" };
            var dto = new StudentProfileDto
            {
                DOB = new DateTime(2000, 1, 1),
                GenderId = 1,
                PhoneNumber = "1234567890",
                AlternatePhoneNumber = "9876543210",
                Address = "Addr 1",
                PermanentAddress = "Addr 2",
                City = "City",
                District = "District",
                State = "State",
                Pincode = "600001",
                IdProofTypeId = 1,
                IdProofNumber = "ID123",
                CourseId = 1
            };

            // Act
            var (success, message, student) = await _repository.SubmitProfileAsync(user, dto);

            // Assert
            Assert.True(success);
            Assert.Equal("Profile submitted successfully", message);
            Assert.NotNull(student);
            Assert.Equal("REG123", student!.RegisterNo);
        }

        [Fact]
        public async Task SubmitProfileAsync_UpdatesProfile_WhenExists()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user2", RegisterNo = "REG456" };

            var existingStudent = new Student
            {
                UserId = user.Id,
                RegisterNo = user.RegisterNo,
                DOB = new DateTime(1999, 1, 1),
                GenderId = 1,
                PhoneNumber = "0000000000",
                Address = "Old Address",
                PermanentAddress = "Old PAddr",
                City = "Old City",
                District = "Old District",
                State = "Old State",
                Pincode = "111111",
                IdProofTypeId = 1,
                IdProofNumber = "OLDID",
                CourseId = 1,
                StatusId = 1
            };

            _context.Students.Add(existingStudent);
            await _context.SaveChangesAsync();

            var dto = new StudentProfileDto
            {
                DOB = new DateTime(2001, 5, 5),
                GenderId = 2,
                PhoneNumber = "2222222222",
                AlternatePhoneNumber = "9999999999",
                Address = "New Address",
                PermanentAddress = "New PAddr",
                City = "New City",
                District = "New District",
                State = "New State",
                Pincode = "222222",
                IdProofTypeId = 2,
                IdProofNumber = "NEWID",
                CourseId = 2
            };

            // Act
            var (success, message, student) = await _repository.SubmitProfileAsync(user, dto);

            // Assert
            Assert.True(success);
            Assert.Equal("Profile submitted successfully", message);
            Assert.NotNull(student);
            Assert.Equal("New Address", student!.Address);
            Assert.Equal("2222222222", student.PhoneNumber);
            Assert.Equal(2, student.GenderId);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
