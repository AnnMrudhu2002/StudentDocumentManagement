using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class StudentDocumentDto
    {
        public int DocumentId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime UploadedOn { get; set; }
        public string FileName { get; set; } = string.Empty;
    }
}
