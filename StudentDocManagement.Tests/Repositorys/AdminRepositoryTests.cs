using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Tests
{
    public class AdminRepositoryTests
    {
        private readonly AppDbContext _context;// In-memory database context
        private readonly AdminRepository _repository;// Repository under test

        public AdminRepositoryTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new AdminRepository(_context);

            // Seed initial test data
            SeedData();
        }

        private void SeedData()
        {
            // Seed supporting entities
            var course = new Course { CourseId = 1, CourseName = "CSE" };
            var statusPending = new StatusMaster { StatusId = 1, StatusName = "Pending" };
            var statusApproved = new StatusMaster { StatusId = 2, StatusName = "Approved" };
            var gender = new Gender { GenderId = 1, Name = "Male" };

            // Seed user
            var user = new ApplicationUser
            {
                Id = "user1",
                FullName = "John Doe",
                Email = "john@example.com",
                RegisterNo = "REG123"
            };

            // Seed student linked to user, course, gender, and status
            var student = new Student
            {
                StudentId = 1,
                UserId = user.Id,
                User = user,
                CourseId = course.CourseId,
                Course = course,
                StatusId = statusPending.StatusId,
                Status = statusPending,
                GenderId = gender.GenderId,
                Gender = gender,
                DOB = new DateTime(2000, 1, 1),
                PhoneNumber = "1234567890",
                AlternatePhoneNumber = "0987654321",
                Address = "123 Main St",
                PermanentAddress = "456 Main St",
                City = "CityX",
                District = "DistrictY",
                State = "StateZ",
                Pincode = "123456",
                IdProofTypeId = 1,
                IdProofNumber = "ID123"
            };

            // Seed document linked to student
            var document = new Document
            {
                DocumentId = 1,
                StudentId = student.StudentId,
                Student = student,
                StatusId = statusPending.StatusId,
                Status = statusPending,
                DocumentTypeId = 1,
                DocumentType = new DocumentType { DocumentTypeId = 1, TypeName = "ID Proof" },
                Remarks = "Test doc"
            };

            // Seed student education
            var education = new StudentEducation
            {
                EducationId = 1,
                StudentId = student.StudentId,
                EducationLevel = "High School",
                InstituteName = "ABC School",
                PassingYear = 2020,
                MarksPercentage = 85
            };

            // Add all entities to context
            _context.Courses.Add(course);
            _context.StatusMasters.AddRange(statusPending, statusApproved);
            _context.Genders.Add(gender);
            _context.Users.Add(user);
            _context.Students.Add(student);
            _context.Documents.Add(document);
            _context.StudentEducations.Add(education);

            _context.SaveChanges();
        }

        #region Tests

        [Fact]
        public async Task GetStudentsForApprovalAsync_ReturnsStudentsWithPendingDocs()
        {
            // Act
            var result = await _repository.GetStudentsForApprovalAsync();

            // Assert: only one student with pending docs
            Assert.Single(result);
            var student = result.First();
            Assert.Equal("John Doe", student.FullName);
            Assert.Equal("REG123", student.RegisterNo);
        }

        [Fact]
        public async Task GetDocumentsByStudentIdAsync_ReturnsPendingDocuments()
        {
            // Act
            var result = await _repository.GetDocumentsByStudentIdAsync(1);

            // Assert: document details match seed data
            Assert.Single(result);
            var doc = result.First();
            Assert.Equal("ID Proof", doc.DocumentTypeName);
            Assert.Equal("Pending", doc.StatusName);
            Assert.Equal("Test doc", doc.Remarks);
        }

        [Fact]
        public async Task UpdateDocumentStatusAsync_UpdatesStatusSuccessfully()
        {
            // Act: update document status to Approved
            var success = await _repository.UpdateDocumentStatusAsync(1, 2, "Approved");
            var doc = await _context.Documents.FindAsync(1);

            // Assert: status updated and remarks set
            Assert.True(success);
            Assert.Equal(2, doc.StatusId);
            Assert.Equal("Approved", doc.Remarks);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ReturnsStudentWithCourseAndGender()
        {
            // Act
            var student = await _repository.GetStudentByIdAsync(1);

            // Assert: student, course, and gender details retrieved correctly
            Assert.NotNull(student);
            Assert.Equal("John Doe", student.User.FullName);
            Assert.Equal("CSE", student.Course.CourseName);
            Assert.Equal("Male", student.Gender.Name);
        }

        [Fact]
        public async Task GetStudentEducationsAsync_ReturnsEducationList()
        {
            // Act
            var educations = await _repository.GetStudentEducationsAsync(1);

            // Assert: education details match seed data
            Assert.Single(educations);
            var edu = educations.First();
            Assert.Equal("High School", edu.EducationLevel);
            Assert.Equal("ABC School", edu.InstituteName);
            Assert.Equal(2020, edu.PassingYear);
            Assert.Equal(85, edu.MarksPercentage);
        }

        [Fact]
        public async Task GetFilteredStudentsAsync_FiltersByBranchNameRegisterNo()
        {
            // Act: filter by branch, name, and register number
            var students = await _repository.GetFilteredStudentsAsync("CSE", "John", "REG123");

            // Assert: only one student matches filter
            Assert.Single(students);
            var student = students.First();
            Assert.Equal("John Doe", student.FullName);
            Assert.Equal("CSE", student.Branch);
            Assert.Equal("REG123", student.RegisterNo);
        }

        [Fact]
        public async Task GetStudentProfileAsync_ReturnsProfileDto()
        {
            // Act: retrieve student profile
            var profile = await _repository.GetStudentProfileAsync(1);

            // Assert: profile DTO contains all expected details
            Assert.NotNull(profile);
            Assert.Equal("John Doe", profile.FullName);
            Assert.Equal("john@example.com", profile.Email);
            Assert.Single(profile.Educations);
            Assert.Equal("High School", profile.Educations.First().EducationLevel);
            Assert.Equal("CSE", profile.CourseName);
            Assert.Equal("Male", profile.GenderName);
        }

        #endregion
    }
}
