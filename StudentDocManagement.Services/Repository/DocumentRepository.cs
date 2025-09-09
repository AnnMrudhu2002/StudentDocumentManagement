using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentDocManagement.Entity.Models;
using StudentDocManagement.Services.Interface;

namespace StudentDocManagement.Services.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Document> UploadAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Status)
                .Include(d => d.Student)
                .FirstOrDefaultAsync(d => d.DocumentId == id);
        }

        public async Task<IEnumerable<Document>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Status)
                .Where(d => d.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int documentId, int statusId, string? remarks)
        {
            var doc = await _context.Documents.FindAsync(documentId);
            if (doc == null) return false;

            doc.StatusId = statusId;
            doc.Remarks = remarks;
            if (statusId == 2) // Example: 2 = Approved
                doc.ApprovedOn = DateTime.UtcNow;

            _context.Documents.Update(doc);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
