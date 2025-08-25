using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
