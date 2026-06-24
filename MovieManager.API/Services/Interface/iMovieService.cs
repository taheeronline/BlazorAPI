using MovieManager.API.DTOs.MovieDTOs;
using MovieManager.API.Wrapper;

namespace MovieManager.API.Services.Interface
{
    public interface iMovieService
    {
        Task<PagedResult<MovieDTO>> GetAll(int page, int pageSize);
        Task<MovieDTO> GetById(int id);
        Task<MovieDTO> CreateMovie(CreateMovieDTO createMovieDTO);
        Task<MovieDTO> UpdateMovie(int id, UpdateMovieDTO updateMovieDTO);
        Task DeleteMovie(int id, int modifiedById);

        Task<PagedResult<MovieDTO>> GetByTitle(string title, int page, int pageSize);
        Task<PagedResult<MovieDTO>> GetByDirector(string director, int page, int pageSize);
        Task<PagedResult<MovieDTO>> GetByGenre(string genre, int page, int pageSize);
    }
}