using StudentDocManagement.Entity.Models;

namespace StudentDocManagement.Services.Interface
{
    public interface ITokenRepository
    {
        Task<string> GetToken(ApplicationUser user, IList<string> roles);
    }
}
