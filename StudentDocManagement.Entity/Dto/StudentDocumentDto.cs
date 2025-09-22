namespace StudentDocManagement.Entity.Dto
{
    public class StudentDocumentDto
    {
        public int DocumentId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime UploadedOn { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int DocumentTypeId { get; set; }
    }
}
