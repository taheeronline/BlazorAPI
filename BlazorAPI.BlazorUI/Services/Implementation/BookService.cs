using System.Net.Http.Json;
using BlazorAPI.BlazorUI.DTOs.BookDTO;
using BlazorAPI.BlazorUI.Services.Interface;
using BlazorAPI.BlazorUI.Wrapper;

namespace BlazorAPI.BlazorUI.Services.Implementation
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;

        public BookService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<PagedResult<BookDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<BookDTO>>(
                $"api/books?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<BookDTO>();
        }

        public async Task<BookDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var book = await _httpClient.GetFromJsonAsync<BookDTO>($"api/books/{id}", cancellationToken);
            return book ?? new BookDTO();
        }

        public async Task<BookDTO> CreateBookAsync(CreateBookDTO createBookDTO, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/books", createBookDTO, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var createdBook = await response.Content.ReadFromJsonAsync<BookDTO>(cancellationToken: cancellationToken);
                return createdBook ?? new BookDTO();
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }

        public async Task<BookDTO> UpdateBookAsync(int id, UpdateBookDTO updateBookDTO, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/books/{id}", updateBookDTO, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    return await response.Content.ReadFromJsonAsync<BookDTO>(cancellationToken: cancellationToken) ?? new BookDTO();
                }
                return new BookDTO();
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }

        public async Task DeleteBookAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/books/{id}?modifiedById={modifiedById}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task<PagedResult<BookDTO>> GetByTitleAsync(string title, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<BookDTO>>(
                $"api/books/search/title/{Uri.EscapeDataString(title)}?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<BookDTO>();
        }

        public async Task<PagedResult<BookDTO>> GetByAuthorAsync(string author, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<BookDTO>>(
                $"api/books/search/author/{Uri.EscapeDataString(author)}?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<BookDTO>();
        }

        public async Task<PagedResult<BookDTO>> GetByPublisherAsync(string publisher, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<BookDTO>>(
                $"api/books/search/publisher/{Uri.EscapeDataString(publisher)}?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<BookDTO>();
        }
    }
}