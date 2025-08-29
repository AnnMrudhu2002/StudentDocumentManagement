using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
 
    [Table("Student")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]   
        public ApplicationUser? User { get; set; }

        [Required, StringLength(20)]
        public string RegisterNo { get; set; } = string.Empty;

        public DateTime DOB { get; set; }
        public string Gender { get; set; } = string.Empty;

        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(15)]
        public string? AlternatePhoneNumber { get; set; }

        [StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [StringLength(300)]
        public string PermanentAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;

        public int IdProofTypeId { get; set; }
        public IdProofType? IdProofType { get; set; }

        [StringLength(50)]
        public string IdProofNumber { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public int StatusId { get; set; }
        public StatusMaster? Status { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        public ICollection<StudentEducation> StudentEducations { get; set; } = new List<StudentEducation>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

}
