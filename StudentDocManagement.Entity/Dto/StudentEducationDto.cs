using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Dto
{
    public class StudentEducationDto
    {
        [Required]
        [StringLength(50)]
        public string EducationLevel { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string InstituteName { get; set; } = string.Empty;

        [Required]
        [MaxYear(ErrorMessage = "Passing Year cannot be in the future")]
        public int PassingYear { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Marks Percentage must be between 0 and 100")]
        public decimal MarksPercentage { get; set; }

    }


    public class StudentEducationListDto
    {
        [Required]
        public List<StudentEducationDto> EducationDetails { get; set; } = new();
    }


    public class MaxYearAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is int year)
            {
                return year <= DateTime.Now.Year;
            }
            return true;
        }
    }

}
