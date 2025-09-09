using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        //public int DocumentId { get; set; }
        //public Document? Document { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }

}
