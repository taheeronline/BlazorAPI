namespace BlazorAPI.API.Models
{
    public class Document : EntityBase
    {
        public string Name { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long FileSize { get; private set; }
        public byte[] FileContent { get; private set; } = Array.Empty<byte>();

        // Make sure these are strictly 'User'
        public User? CreatedByUser { get; private set; }
        public User? ModifiedByUser { get; private set; }

        public Document()
        {
            Name = string.Empty;
            FileName = string.Empty;
            ContentType = string.Empty;
            FileSize = 0;
            FileContent = Array.Empty<byte>();
        }

        public Document(string name, string fileName, string contentType, long fileSize, byte[] fileContent)
        {
            Name = name;
            FileName = fileName;
            ContentType = contentType;
            FileSize = fileSize;
            FileContent = fileContent;
        }

        public static Document Create(string name, string fileName, string contentType, long fileSize, byte[] fileContent, int createdBy)
        {
            ValidateInputs(name, fileName, contentType, fileSize, fileContent);
            if (createdBy <= 0) throw new ArgumentException("Invalid User ID.", nameof(createdBy));

            var document = new Document(name, fileName, contentType, fileSize, fileContent)
            {
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = createdBy
            };
            return document;
        }

        public void Update(string name, string fileName, string contentType, long fileSize, byte[] fileContent)
        {
            ValidateInputs(name, fileName, contentType, fileSize, fileContent);

            if (Id <= 0) throw new ArgumentException("Invalid Document ID.", nameof(Id));

            this.Name = name;
            this.FileName = fileName;
            this.ContentType = contentType;
            this.FileSize = fileSize;
            this.FileContent = fileContent;

            UpdateLastModified();
        }

        private static void ValidateInputs(string name, string fileName, string contentType, long fileSize, byte[] fileContent)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File Name cannot be null or empty.", nameof(fileName));
            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content Type cannot be null or empty.", nameof(contentType));
            if (fileSize < 0)
                throw new ArgumentOutOfRangeException(nameof(fileSize), "File Size cannot be negative.");
            if (fileContent == null || fileContent.Length == 0)
                throw new ArgumentException("File Content cannot be null or empty.", nameof(fileContent));
        }
    }
}