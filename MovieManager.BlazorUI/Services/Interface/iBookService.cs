using MovieManager.BlazorUI.DTOs.BookDTO;

namespace MovieManager.BlazorUI.Services.Interface
{
    public interface iBookService
    {
        Task<IEnumerable<BookDTO>> GetAll();
        Task<BookDTO> GetById(int id);
        Task<BookDTO> CreateBook(CreateBookDTO createBookDTO);
        Task<BookDTO> UpdateBook(int id, UpdateBookDTO updateBookDTO);
        Task DeleteBook(int id, int modifiedById);
        Task<IEnumerable<BookDTO>> GetByTitle(string title);
        Task<IEnumerable<BookDTO>> GetByAuthor(string author);
        Task<IEnumerable<BookDTO>> GetByPublisher(string publisher);
    }
}
