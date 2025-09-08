using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Range(1900, 2100, ErrorMessage = "Passing Year must be between 1900 and 2100")]
        public int PassingYear { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Marks Percentage must be between 0 and 100")]
        public decimal MarksPercentage { get; set; }

    }
}
