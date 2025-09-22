using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Models
{
    public class StatusMaster
    {
        [Key]
        public int StatusId { get; set; }

        [Required, StringLength(50)]
        public string StatusName { get; set; } = string.Empty;  // e.g. Pending, Approved, Rejected, Changes Needed
    }

}
