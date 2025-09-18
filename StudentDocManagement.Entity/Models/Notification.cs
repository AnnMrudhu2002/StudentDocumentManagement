using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required, StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }

}
