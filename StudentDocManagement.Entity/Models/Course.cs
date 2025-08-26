using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
    public class Course
    {
        [Key]
        //primary key
        public int CourseId { get; set; }

        [Required, StringLength(200)]
        public string CourseName { get; set; } = string.Empty;
    }

}
