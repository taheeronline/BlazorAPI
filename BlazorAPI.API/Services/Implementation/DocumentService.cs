using Microsoft.EntityFrameworkCore;
using BlazorAPI.API.DTOs.DocumentDTO;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Models;
using BlazorAPI.API.Persistence;
using BlazorAPI.API.Services.Interface;

namespace BlazorAPI.API.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(MyDbContext context, ILogger<DocumentService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<DocumentMetadataDto>> GetAllMetadataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var documents = await _context.Documents
                    .AsNoTracking()
                    // Fix applied here
                    .Where(d => !(d.IsDeleted ?? false))
                    .ToListAsync(cancellationToken);

                return documents.Select(MapToMetadataDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document metadata.");
                throw new DocumentExceptions("Failed to retrieve documents. Please try again later.", 500);
            }
        }

        public async Task<DocumentMetadataDto> GetMetadataByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "Document");

            try
            {
                var document = await _context.Documents
                    .AsNoTracking()
                    // Fix applied here
                    .FirstOrDefaultAsync(d => d.Id == id && !(d.IsDeleted ?? false), cancellationToken);

                if (document is null) throw new DocumentNotFoundException(id);

                return MapToMetadataDto(document);
            }
            catch (DocumentNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document metadata with ID {DocumentId}.", id);
                throw new DocumentExceptions("Failed to retrieve document metadata.", 500);
            }
        }

        public async Task<DocumentFullDto> GetFullDocumentByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "Document");

            try
            {
                var document = await _context.Documents
                    .AsNoTracking()
                    // Fix applied here
                    .FirstOrDefaultAsync(d => d.Id == id && !(d.IsDeleted ?? false), cancellationToken);

                if (document is null) throw new DocumentNotFoundException(id);

                return MapToFullDto(document);
            }
            catch (DocumentNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving full document with ID {DocumentId}.", id);
                throw new DocumentExceptions("Failed to retrieve full document.", 500);
            }
        }

        public async Task<DocumentMetadataDto> CreateDocumentAsync(DocumentCreateDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            try
            {
                // TODO: Retrieve the actual logged-in user's ID from IHttpContextAccessor
                int currentUserId = 1;

                var document = Document.Create(
                    name: dto.Name,
                    fileName: dto.FileName,
                    contentType: dto.ContentType,
                    fileSize: dto.FileContent.Length,
                    fileContent: dto.FileContent,
                    createdBy: currentUserId
                );

                _context.Documents.Add(document);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document created with ID {DocumentId}.", document.Id);
                return MapToMetadataDto(document);
            }
            catch (ArgumentException ex)
            {
                throw new DocumentValidationException($"Document validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document.");
                throw new DocumentExceptions("Failed to create document.", 500);
            }
        }

        public async Task UpdateDocumentAsync(DocumentUpdateDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            GuardPositiveId(dto.Id, "Document");

            try
            {
                var document = await _context.Documents
                    // Fix applied here
                    .FirstOrDefaultAsync(d => d.Id == dto.Id && !(d.IsDeleted ?? false), cancellationToken);

                if (document is null) throw new DocumentNotFoundException(dto.Id);

                bool hasNewFile = dto.FileContent != null && dto.FileContent.Length > 0;

                string fileNameToUpdate = hasNewFile ? dto.FileName! : document.FileName;
                string contentTypeToUpdate = hasNewFile ? dto.ContentType! : document.ContentType;
                byte[] fileContentToUpdate = hasNewFile ? dto.FileContent! : document.FileContent;
                long fileSizeToUpdate = hasNewFile ? dto.FileContent!.Length : document.FileSize;

                document.Update(
                    name: dto.Name,
                    fileName: fileNameToUpdate,
                    contentType: contentTypeToUpdate,
                    fileSize: fileSizeToUpdate,
                    fileContent: fileContentToUpdate
                );

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document with ID {DocumentId} updated.", dto.Id);
            }
            catch (DocumentNotFoundException) { throw; }
            catch (ArgumentException ex)
            {
                throw new DocumentValidationException($"Document validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID {DocumentId}.", dto.Id);
                throw new DocumentExceptions("Failed to update document.", 500);
            }
        }

        public async Task DeleteDocumentAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "Document");
            GuardPositiveId(modifiedById, "ModifiedBy");

            try
            {
                var document = await _context.Documents
                    // Fix applied here
                    .FirstOrDefaultAsync(d => d.Id == id && !(d.IsDeleted ?? false), cancellationToken);

                if (document is null) throw new DocumentNotFoundException(id);

                document.MarkAsDeleted(modifiedById);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document with ID {DocumentId} soft-deleted by user {UserId}.", id, modifiedById);
            }
            catch (DocumentNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document with ID {DocumentId}.", id);
                throw new DocumentExceptions("Failed to delete document.", 500);
            }
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private static DocumentMetadataDto MapToMetadataDto(Document document) => new()
        {
            Id = document.Id,
            Name = document.Name,
            FileName = document.FileName,
            ContentType = document.ContentType,
            FileSize = document.FileSize
        };

        private static DocumentFullDto MapToFullDto(Document document) => new()
        {
            Id = document.Id,
            Name = document.Name,
            FileName = document.FileName,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            FileContent = document.FileContent
        };

        private static void GuardPositiveId(int id, string paramName)
        {
            if (id <= 0) throw new DocumentValidationException($"{paramName} ID must be greater than zero.");
        }
    }
}