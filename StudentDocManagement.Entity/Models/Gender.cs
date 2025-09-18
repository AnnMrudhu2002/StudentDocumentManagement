using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
    [Table("Gender")]
    public class Gender
    {
        [Key]
        public int GenderId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property 
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
