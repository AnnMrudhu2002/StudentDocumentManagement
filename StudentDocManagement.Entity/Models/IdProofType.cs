using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Models
{
    public class IdProofType
    {
        [Key]
        public int IdProofTypeId { get; set; }

        [Required, StringLength(100)]
        public string TypeName { get; set; } = string.Empty; // Aadhaar, Passport etc.
    }

}
