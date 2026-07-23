using System.Net.Http.Json;
using BlazorAPI.BlazorUI.DTOs.DocumentDTO;
using BlazorAPI.BlazorUI.Services.Interface;

namespace BlazorAPI.BlazorUI.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly HttpClient _httpClient;

        public DocumentService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<DocumentMetadataDto>> GetAllMetadataAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<DocumentMetadataDto>>(
                "api/documents",
                cancellationToken);

            return response ?? Array.Empty<DocumentMetadataDto>();
        }

        public async Task<DocumentMetadataDto> GetMetadataByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var document = await _httpClient.GetFromJsonAsync<DocumentMetadataDto>(
                $"api/documents/{id}",
                cancellationToken);

            return document ?? new DocumentMetadataDto();
        }

        public async Task<DocumentFullDto> GetFullDocumentByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var document = await _httpClient.GetFromJsonAsync<DocumentFullDto>(
                $"api/documents/{id}/full",
                cancellationToken);

            return document ?? new DocumentFullDto();
        }

        public async Task<DocumentMetadataDto> CreateDocumentAsync(DocumentCreateDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/documents", dto, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var createdDocument = await response.Content.ReadFromJsonAsync<DocumentMetadataDto>(cancellationToken: cancellationToken);
                return createdDocument ?? new DocumentMetadataDto();
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }

        public async Task UpdateDocumentAsync(DocumentUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/documents/{dto.Id}", dto, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task DeleteDocumentAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/documents/{id}?modifiedById={modifiedById}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }
    }
}