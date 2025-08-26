using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StudentDocManagement.Entity.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        public int DocumentTypeId { get; set; }
        public DocumentType? DocumentType { get; set; }

        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public int StatusId { get; set; }
        public StatusMaster? Status { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        //example
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedOn { get; set; }
    }


}
