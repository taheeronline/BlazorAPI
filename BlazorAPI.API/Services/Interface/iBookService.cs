using BlazorAPI.API.DTOs.BookDTO;

namespace BlazorAPI.API.Services.Interface
{
    public interface iBookService
    {
        public Task<IEnumerable<BookDTO>> GetAll();
        public Task<BookDTO> GetById(int id);
        public Task<BookDTO> CreateBook(CreateBookDTO createBookDTO);
        public Task<BookDTO> UpdateBook(int id, UpdateBookDTO updateBookDTO);
        public Task<bool> DeleteBook(int id, int modifiedById);
        public Task<IEnumerable<BookDTO>> GetByTitle(string title);
        public Task<IEnumerable<BookDTO>> GetByAuthor(string author);
        public Task<IEnumerable<BookDTO>> GetByPublisher(string publisher);

    }
}
