namespace StudentDocManagement.Entity.Dto
{
    public class StudentStatusUpdateDto
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}
