using BlazorAPI.BlazorUI.DTOs.BookDTO;
using BlazorAPI.BlazorUI.Services.Interface;

namespace BlazorAPI.BlazorUI.Services.Implementation
{
    public class BookService : iBookService
    {
        private readonly HttpClient _httpClient;

        public BookService( HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<BookDTO> CreateBook(CreateBookDTO createBookDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("api/books", createBookDTO);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BookDTO>();
        }

        public async Task<BookDTO> UpdateBook(int id, UpdateBookDTO updateBookDTO)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/books/{id}", updateBookDTO);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BookDTO>();
        }

        public Task DeleteBook(int id, int modifiedById)
        {
            return _httpClient.DeleteAsync($"api/books/{id}?modifiedById={modifiedById}");
        }

        public async Task<IEnumerable<BookDTO>> GetAll()
        {
            var books = await _httpClient.GetFromJsonAsync<IEnumerable<BookDTO>>("api/books");
            return books ?? Enumerable.Empty<BookDTO>();
        }

        public async Task<IEnumerable<BookDTO>> GetByAuthor(string author)
        {
            var books = await _httpClient.GetFromJsonAsync<IEnumerable<BookDTO>>($"api/books/search/author/{author}");
            return books ?? Enumerable.Empty<BookDTO>();
        }

        public async Task<BookDTO> GetById(int id)
        {
            var book = await _httpClient.GetFromJsonAsync<BookDTO>($"api/books/{id}");
            return book ?? new BookDTO();
        }

        public async Task<IEnumerable<BookDTO>> GetByPublisher(string publisher)
        {
            var books = await _httpClient.GetFromJsonAsync<IEnumerable<BookDTO>>($"api/books/search/publisher/{publisher}");
            return books ?? Enumerable.Empty<BookDTO>();
        }

        public async Task<IEnumerable<BookDTO>> GetByTitle(string title)
        {
            var books = await _httpClient.GetFromJsonAsync<IEnumerable<BookDTO>>($"api/books/search/title/{title}");
            return books ?? Enumerable.Empty<BookDTO>();
        }

    }
}
