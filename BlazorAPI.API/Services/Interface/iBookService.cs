using BlazorAPI.API.DTOs.BookDTO;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Services.Interface
{
    public interface IBookService
    {
        Task<PagedResult<BookDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<BookDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<BookDTO> CreateBookAsync(CreateBookDTO createBookDTO, CancellationToken cancellationToken = default);
        Task<BookDTO> UpdateBookAsync(int id, UpdateBookDTO updateBookDTO, CancellationToken cancellationToken = default);
        Task DeleteBookAsync(int id, int modifiedById, CancellationToken cancellationToken = default);
        Task<PagedResult<BookDTO>> GetByTitleAsync(string title, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<BookDTO>> GetByAuthorAsync(string author, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<BookDTO>> GetByPublisherAsync(string publisher, int page, int pageSize, CancellationToken cancellationToken = default);

    }
}
