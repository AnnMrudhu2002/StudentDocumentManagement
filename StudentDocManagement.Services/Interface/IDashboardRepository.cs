using StudentDocManagement.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Interface
{
    public interface IDashboardRepository
    {
        Task<decimal> GetProfileCompletionAsync(Student student);
    }
}
