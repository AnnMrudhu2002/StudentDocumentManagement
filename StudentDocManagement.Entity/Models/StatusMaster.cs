using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
    public class StatusMaster
    {
        [Key]
        public int StatusId { get; set; }

        [Required, StringLength(50)]
        public string StatusType { get; set; } = string.Empty;  // e.g. UserAccount, Application, Document

        [Required, StringLength(50)]
        public string StatusName { get; set; } = string.Empty;  // e.g. Pending, Approved, Rejected
    }

}
