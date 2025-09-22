using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Dto
{
    public class CreateRoleDto
    {
        [Required]
        public string RoleName { get; set; }
    }
}
