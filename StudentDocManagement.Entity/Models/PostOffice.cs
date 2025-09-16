using System.ComponentModel.DataAnnotations;
using StudentDocManagement.Entity.Models;

public class PostOffices
{
    [Key]
    public int OfficeId { get; set; }

    public string OfficeName { get; set; }

    public int PincodeId { get; set; }
    public Pincode Pincode { get; set; }
}
