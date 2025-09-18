using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class ProfilePageDto
    {


            public DateTime DOB { get; set; }
            //public string Gender { get; set; } = string.Empty;
            public int GenderId { get; set; }
        public string GenderName { get; set; } = string.Empty;
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
            public string FullName { get; set; }
            public string Email { get; set; }
            public string CourseName { get; set; }
        [JsonPropertyName("educations")]
        public List<StudentEducationDto> Educations { get; set; } = new();
    }
    }
