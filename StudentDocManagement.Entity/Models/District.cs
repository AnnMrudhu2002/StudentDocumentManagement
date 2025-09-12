using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
    public class District
    {
        [Key]
        public long DistrictId { get; set; }

        [Required, MaxLength(100)]
        public string DistrictName { get; set; } = string.Empty;

        // Foreign key to State
        public long StateId { get; set; }
        [ForeignKey("StateId")]
        //public State? State { get; set; }

        public ICollection<PostOffice> PostOffices { get; set; } = new List<PostOffice>();
    }
}
