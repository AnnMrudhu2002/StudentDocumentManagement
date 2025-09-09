using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class CreateRoleDto
    {
        [Required]
        public string RoleName { get; set; }
    }
}
