using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class StudentProfileDto
    {

        public DateTime DOB { get; set; }
        public int GenderId { get; set; } 
        public string? Name { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;


        public int IdProofTypeId { get; set; }
        public string IdProofNumber { get; set; } = string.Empty;
        public int CourseId { get; set; }
        
    }
}
