using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Services.Interface
{
    public interface ITokenRepository
    {
        Task<string> GetToken(ApplicationUser user, IList<string> roles);
    }
}
