namespace StudentDocManagement.Entity.Models
{
    public class District
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }

        public int StateId { get; set; }
        public State State { get; set; }

        public ICollection<Pincode> Pincodes { get; set; }
    }
}
