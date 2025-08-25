using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace StudentDocManagement.Entity.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string RegisterNo { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // ✅ Needed because property name doesn't match navigation name
        [ForeignKey(nameof(AccountStatus))]
        public int AccountStatusId { get; set; }
        public StatusMaster? AccountStatus { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }

}
