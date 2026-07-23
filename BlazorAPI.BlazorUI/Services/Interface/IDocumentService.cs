using BlazorAPI.BlazorUI.DTOs.DocumentDTO;
using BlazorAPI.BlazorUI.Wrapper;

namespace BlazorAPI.BlazorUI.Services.Interface
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentMetadataDto>> GetAllMetadataAsync(CancellationToken cancellationToken = default);

        Task<DocumentMetadataDto> GetMetadataByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<DocumentFullDto> GetFullDocumentByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<DocumentMetadataDto> CreateDocumentAsync(DocumentCreateDto dto, CancellationToken cancellationToken = default);

        Task UpdateDocumentAsync(DocumentUpdateDto dto, CancellationToken cancellationToken = default);

        Task DeleteDocumentAsync(int id, int modifiedById, CancellationToken cancellationToken = default);
    }
}