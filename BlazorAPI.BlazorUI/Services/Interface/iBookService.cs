using BlazorAPI.BlazorUI.DTOs.BookDTO;
using BlazorAPI.BlazorUI.Wrapper;

namespace BlazorAPI.BlazorUI.Services.Interface
{
    public interface IBookService
    {
        Task<PagedResult<BookDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<BookDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<BookDTO> CreateBookAsync(CreateBookDTO createBookDTO, CancellationToken cancellationToken = default);
        Task<BookDTO> UpdateBookAsync(int id, UpdateBookDTO updateBookDTO, CancellationToken cancellationToken = default);
        Task DeleteBookAsync(int id, int modifiedById, CancellationToken cancellationToken = default);

        Task<PagedResult<BookDTO>> GetByTitleAsync(string title, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<BookDTO>> GetByAuthorAsync(string author, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<BookDTO>> GetByPublisherAsync(string publisher, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    }
}