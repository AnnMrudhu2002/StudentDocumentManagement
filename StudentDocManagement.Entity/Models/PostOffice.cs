using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
    public class PostOffices
    {
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }

        public int PincodeId { get; set; }
        public Pincode Pincode { get; set; }
    }
}
