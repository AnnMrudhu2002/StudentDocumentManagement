namespace StudentDocManagement.Entity.Models
{
    public class State
    {
        public int StateId { get; set; }
        public string StateName { get; set; }

        public ICollection<District> Districts { get; set; }
    }

}
