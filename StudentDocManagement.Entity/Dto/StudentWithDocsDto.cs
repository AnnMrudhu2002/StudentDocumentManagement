using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class StudentWithDocsDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string RegisterNo { get; set; } = string.Empty;
    }
}
