using BlazorAPI.API.DTOs.MovieDTOs;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Services.Interface
{
    public interface IMovieService
    {

        Task<PagedResult<MovieDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<MovieDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<MovieDTO> CreateMovieAsync(CreateMovieDTO createMovieDTO, CancellationToken cancellationToken = default);
        Task<MovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO updateMovieDTO, CancellationToken cancellationToken = default);
        Task DeleteMovieAsync(int id, int modifiedById, CancellationToken cancellationToken = default);
        Task<PagedResult<MovieDTO>> GetByTitleAsync(string title, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<MovieDTO>> GetByDirectorAsync(string director, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<MovieDTO>> GetByGenreAsync(string genre, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}