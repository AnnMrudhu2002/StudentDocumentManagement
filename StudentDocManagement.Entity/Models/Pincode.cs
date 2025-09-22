namespace StudentDocManagement.Entity.Models
{
    public class Pincode
    {
        public int PincodeId { get; set; }
        public string Code { get; set; }

        public int DistrictId { get; set; }
        public District District { get; set; }

        public ICollection<PostOffices> postOffices { get; set; }
    }
}
