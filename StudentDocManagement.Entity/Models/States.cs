using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Models
{
    public class State
    {
        public int StateId { get; set; }
        public string StateName { get; set; }

        public ICollection<District> Districts { get; set; }
    }

}
