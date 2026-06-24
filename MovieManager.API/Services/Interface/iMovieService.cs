using MovieManager.API.DTOs.MovieDTOs;

namespace MovieManager.API.Services.Interface
{
    public interface iMovieService
    {
        Task<IEnumerable<MovieDTO>> GetAll();
        Task<MovieDTO> GetById(int id);
        Task<MovieDTO> CreateMovie(CreateMovieDTO createMovieDTO);
        Task<MovieDTO> UpdateMovie(int id, UpdateMovieDTO updateMovieDTO);
        Task DeleteMovie(int id, int modifiedById);
        Task<IEnumerable<MovieDTO>> GetByTitle(string title);
        Task<IEnumerable<MovieDTO>> GetByDirector(string director);
        Task<IEnumerable<MovieDTO>> GetByGenre(string genre);
    }
}