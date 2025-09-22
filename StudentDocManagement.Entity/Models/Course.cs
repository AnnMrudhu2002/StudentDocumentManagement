using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required, StringLength(200)]
        public string CourseName { get; set; } = string.Empty;
    }

}
