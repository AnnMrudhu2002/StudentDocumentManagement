using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using StudentDocumentManagement.Controllers;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class AdminDashboardControllerTests
{
    // Helper method to create an in-memory DbContext with seeded data
    private AppDbContext GetInMemoryDbContext()
    {
        // Setup in-memory database options
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AdminDashboardTestDb")
            .Options;

        var context = new AppDbContext(options);

        // Seed Users with different statuses
        context.Users.AddRange(new List<ApplicationUser>
        {
            new ApplicationUser { Id = "1", StatusId = 1, FullName = "Pending User 1" }, // pending
            new ApplicationUser { Id = "2", StatusId = 2, FullName = "Approved User" },  // approved
            new ApplicationUser { Id = "3", StatusId = 1, FullName = "Pending User 2" }  // pending
        });

        // Seed Students
        var student1 = new Student { StudentId = 1, RegisterNo = "R001" };
        var student2 = new Student { StudentId = 2, RegisterNo = "R002" };
        context.Students.AddRange(student1, student2);

        // Seed Documents with different statuses
        context.Documents.AddRange(new List<Document>
        {
            new Document { DocumentId = 1, Student = student1, StatusId = 1 }, // pending document
            new Document { DocumentId = 2, Student = student1, StatusId = 2 }, // approved document
            new Document { DocumentId = 3, Student = student2, StatusId = 1 }  // pending document
        });

        // Save all seeded data
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetDashboardSummary_ReturnsCorrectCounts()
    {
        // Arrange: create in-memory DB and controller
        var context = GetInMemoryDbContext();
        var controller = new AdminDashboardController(context);

        // Act: call the method to get dashboard summary
        var result = await controller.GetDashboardSummary() as OkObjectResult;
        Assert.NotNull(result); // Ensure result is not null

        // Convert anonymous object returned by controller to a dictionary for easy access
        var data = result.Value
            .GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(result.Value));

        // Assert: check that counts are correct based on seeded data
        Assert.Equal(2, (int)data["PendingStudents"]); // 2 pending users
        Assert.Equal(2, (int)data["StudentsWithPendingDocuments"]); // 2 students have pending documents
    }

    [Fact]
    public async Task GetDashboardSummary_EmptyDatabase_ReturnsZero()
    {
        // Arrange: create empty in-memory DB
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "EmptyDb")
            .Options;
        var context = new AppDbContext(options);
        var controller = new AdminDashboardController(context);

        // Act: call dashboard summary method
        var result = await controller.GetDashboardSummary() as OkObjectResult;
        Assert.NotNull(result); // Ensure result is not null

        // Convert anonymous result to dictionary
        var data = result.Value
            .GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(result.Value));

        // Assert: with no data, counts should be zero
        Assert.Equal(0, (int)data["PendingStudents"]);
        Assert.Equal(0, (int)data["StudentsWithPendingDocuments"]);
    }
}
