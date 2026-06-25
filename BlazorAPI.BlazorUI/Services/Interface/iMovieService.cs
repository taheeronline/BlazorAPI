using BlazorAPI.BlazorUI.DTOs.MovieDTOs;
using BlazorAPI.BlazorUI.Wrapper;

namespace BlazorAPI.BlazorUI.Services.Interface
{
    public interface iMovieService
    {
        Task<PagedResult<MovieDTO>> GetAll(int page, int pageSize);
        Task<MovieDTO> GetById(int id);
        Task<MovieDTO> CreateMovie(CreateMovieDTO createMovieDTO);
        Task<MovieDTO> UpdateMovie(int id, UpdateMovieDTO updateMovieDTO);
        Task DeleteMovie(int id, int modifiedById);

        Task<PagedResult<MovieDTO>> GetByTitle(string title, int page = 1, int pageSize = 10);
        Task<PagedResult<MovieDTO>> GetByDirector(string director, int page = 1, int pageSize = 10);
        Task<PagedResult<MovieDTO>> GetByGenre(string genre, int page = 1, int pageSize = 10);
    }
}
