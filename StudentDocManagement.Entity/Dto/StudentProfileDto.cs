using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class StudentProfileDto
    {
        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid phone number")]
        public string? AlternatePhoneNumber { get; set; }

        [Required, StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string PermanentAddress { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string District { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid pincode")]
        public string Pincode { get; set; } = string.Empty;

        [Required]
        public int IdProofTypeId { get; set; }

        [Required, StringLength(50)]
        public string IdProofNumber { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }
    }
}
