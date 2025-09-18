using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StudentDocManagement.Entity.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(8)]
        public string RegisterNo { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; }
        public StatusMaster? Status { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }

}
