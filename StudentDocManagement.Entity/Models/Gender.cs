using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentDocManagement.Entity.Models
{
    [Table("Gender")]
    public class Gender
    {
        [Key]
        public int GenderId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

      
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
