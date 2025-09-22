using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Models
{
    public class StudentEducation
    {
        [Key]
        public int EducationId { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [StringLength(50)]
        public string EducationLevel { get; set; } = string.Empty;

        [StringLength(200)]
        public string InstituteName { get; set; } = string.Empty;

        public int PassingYear { get; set; }

    
        public decimal MarksPercentage { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }

}
