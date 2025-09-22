using System.ComponentModel.DataAnnotations;

namespace StudentDocManagement.Entity.Dto
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Invalid password format")]

        public string Password { get; set; } = string.Empty;
    }
}
