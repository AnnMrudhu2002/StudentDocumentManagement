using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class StudentListDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string RegisterNo { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
