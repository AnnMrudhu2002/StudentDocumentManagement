using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class FileUploadDto
    {
        public string FileName { get; set; } = string.Empty;
        public Stream FileStream { get; set; } = default!;
        public int DocumentTypeId { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}
