using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
