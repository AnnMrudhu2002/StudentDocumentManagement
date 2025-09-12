using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
    public class PostOffice
    {
        [Key]
        public long PostOfficeId { get; set; }

        [Required, MaxLength(100)]
        public string PostOfficeName { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;

        // Foreign key to District
        public long DistrictId { get; set; }
        [ForeignKey("DistrictId")]
        public District? District { get; set; }
    }
}
