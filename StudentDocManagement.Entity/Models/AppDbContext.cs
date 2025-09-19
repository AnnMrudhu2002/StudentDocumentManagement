using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StudentDocManagement.Entity.Models
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
  : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<IdProofType> IdProofTypes { get; set; }
        public DbSet<StatusMaster> StatusMasters { get; set; }
        public DbSet<StudentEducation> StudentEducations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<State> states { get; set; }
        public DbSet<District> districts { get; set; }
        public DbSet<Pincode> pincodes { get; set; }
        public DbSet<PostOffices> postOffices { get; set; }
        public DbSet<Gender> Genders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Status)
                .WithMany()
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Disable cascade on Documents → Student
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Student)
                .WithMany(s => s.Documents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cascade delete is fine for Student → ApplicationUser
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint on RegisterNo
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.RegisterNo)
                .IsUnique();

            // Default value for Notification.IsRead
            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);

            modelBuilder.Entity<StudentEducation>()
                .Property(se => se.MarksPercentage)
                .HasPrecision(5, 2); 

            modelBuilder.Entity<StudentEducation>()
                .HasOne(se => se.Student)
                .WithMany(s => s.StudentEducations)
                .HasForeignKey(se => se.StudentId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<State>()
           .HasIndex(s => s.StateName)
              .IsUnique();

            modelBuilder.Entity<District>()
                .HasIndex(d => new { d.StateId, d.DistrictName })
                .IsUnique();

            modelBuilder.Entity<Pincode>()
                .HasIndex(p => new { p.DistrictId, p.PincodeId })
                .IsUnique();

            modelBuilder.Entity<District>()
                .HasOne(d => d.State)
                .WithMany(s => s.Districts)
                .HasForeignKey(d => d.StateId);

            modelBuilder.Entity<Pincode>()
                .HasOne(p => p.District)
                .WithMany(d => d.Pincodes)
                .HasForeignKey(p => p.DistrictId);

            modelBuilder.Entity<PostOffices>()
                .HasOne(po => po.Pincode)
                .WithMany(p => p.postOffices)
                .HasForeignKey(po => po.PincodeId);


            // Seed Courses
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, CourseName = "Computer Science" },
                new Course { CourseId = 2, CourseName = "Information Technology" },
                new Course { CourseId = 3, CourseName = "Electronics and Communication Engineering" },
                new Course { CourseId = 4, CourseName = "Mechanical Engineering" },
                new Course { CourseId = 5, CourseName = "Civil Engineering" },
                new Course { CourseId = 6, CourseName = "Electrical Engineering" },
                new Course { CourseId = 7, CourseName = "Chemical Engineering" },
                new Course { CourseId = 8, CourseName = "Biotechnology" },
                new Course { CourseId = 9, CourseName = "Aeronautical Engineering" },
                new Course { CourseId = 10, CourseName = "Environmental Engineering" }
            );

            // Seed Document Types
            modelBuilder.Entity<DocumentType>().HasData(
                new DocumentType { DocumentTypeId = 1, TypeName = "ID Proof" },
                new DocumentType { DocumentTypeId = 2, TypeName = "10th Certificate" },
                new DocumentType { DocumentTypeId = 3, TypeName = "12th Certificate" }
            );

            // Seed ID Proof Types
            modelBuilder.Entity<IdProofType>().HasData(
                new IdProofType { IdProofTypeId = 1, TypeName = "Aadhar Card" },
                new IdProofType { IdProofTypeId = 2, TypeName = "PAN Card" },
                new IdProofType { IdProofTypeId = 3, TypeName = "Driver's License" },
                new IdProofType { IdProofTypeId = 4, TypeName = "Voters ID" }

            );

     
            // Seed StatusMaster
            modelBuilder.Entity<StatusMaster>().HasData(
                // RegistrationStatus
                new StatusMaster { StatusId = 1, StatusName = "Pending" },
                new StatusMaster { StatusId = 2, StatusName = "Approved" },
                new StatusMaster { StatusId = 3, StatusName = "Rejected" },           
                new StatusMaster { StatusId = 4, StatusName = "Changes Needed" },
                new StatusMaster { StatusId = 5, StatusName = "In Review" }
                

            );

            // Seed Gender

            modelBuilder.Entity<Gender>().HasData(
                new Gender { GenderId = 1, Name = "Male" },
                new Gender { GenderId = 2, Name = "Female" },
                new Gender { GenderId = 3, Name = "Other" }
);



        }

    }
}
