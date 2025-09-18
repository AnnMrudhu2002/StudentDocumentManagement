namespace StudentDocManagement.Entity.Dto
{
    public class FileUploadDto
    {
        public string FileName { get; set; } = string.Empty;
        public Stream FileStream { get; set; } = default!;
        public int DocumentTypeId { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }
}
