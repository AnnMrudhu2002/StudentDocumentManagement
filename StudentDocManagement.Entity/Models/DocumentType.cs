using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Models
{
    public class DocumentType
    {
        [Key]
        public int DocumentTypeId { get; set; }

        [Required, StringLength(100)]
        public string TypeName { get; set; } = string.Empty;
    }

}
