namespace BlazorAPI.API.DTOs.DocumentDTO
{
    public class DocumentMetadataDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
    }

    // Used when the UI specifically requests to preview/edit the file (Heavy)
    public class DocumentFullDto : DocumentMetadataDto
    {
        public byte[] FileContent { get; set; }
    }

    // Used when creating a new document from the UI
    public class DocumentCreateDto
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileContent { get; set; }
    }

    // Used when updating an existing document (e.g., saving after GUI edit)
    public class DocumentUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // These can be nullable. If the user only changes the Name, 
        // they shouldn't have to re-upload the 5MB byte array.
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? FileContent { get; set; }
    }
}
