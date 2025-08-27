using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class StudentStatusUpdateDto
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}
