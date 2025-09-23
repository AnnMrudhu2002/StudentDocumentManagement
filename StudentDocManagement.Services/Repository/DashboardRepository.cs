using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDocManagement.Services.Repository
{
    public class DashboardRepository: IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }


        //compute profile completion
        public async Task<decimal> GetProfileCompletionAsync(Student student)
        {
            decimal completion = 0;

            if (student == null) return 0;

            // Student profile entry
            bool hasProfileData = !string.IsNullOrEmpty(student.RegisterNo)
                                  && !string.IsNullOrEmpty(student.PhoneNumber)
                                  && !string.IsNullOrEmpty(student.Address)
                                  && student.CourseId > 0;
            if (hasProfileData)
                completion += 20;

            // Education entries 
            var educations = student.StudentEducations.ToList();
            if (educations.Count(e => !string.IsNullOrEmpty(e.EducationLevel)
                                     && !string.IsNullOrEmpty(e.InstituteName)) >= 1)
                completion += 10;
            if (educations.Count(e => !string.IsNullOrEmpty(e.EducationLevel)
                                     && !string.IsNullOrEmpty(e.InstituteName)) >= 2)
                completion += 10;

            // Documents 
            var validStatuses = new List<int> { 1, 2 }; // pending or approved
            completion += student.Documents.Count(d => validStatuses.Contains(d.StatusId)) * 15;

            completion = Math.Min(completion, 20 + 20 + 45); // 85 max before acknowledgment

            // Acknowledgement 
            if (student.IsAcknowledged)
                completion += 15;

            // Cap at 100%
            return Math.Min(completion, 100);
        }


    }
}
