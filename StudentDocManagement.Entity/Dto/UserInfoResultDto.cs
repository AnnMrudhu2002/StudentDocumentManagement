using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Entity.Dto
{
    public class UserInfoResultDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RegisterNo { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
    }
}
