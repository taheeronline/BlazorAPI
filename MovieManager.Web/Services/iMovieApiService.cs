using MovieManager.Web.DTOs;

namespace MovieManager.Web.Services
{
    public interface iMovieApiService
    {
        Task<IEnumerable<MovieDTO>> GetAllMoviesAsync();
        Task<MovieDTO?> GetMovieByIdAsync(Guid id);
        Task<MovieDTO?> CreateMovieAsync(CreateMovieDTO movie);
        Task<MovieDTO?> UpdateMovieAsync(Guid id, UpdateMovieDTO movie);
        Task<bool> DeleteMovieAsync(Guid id);

        // Targeted search endpoints matching the API routing
        Task<IEnumerable<MovieDTO>> SearchByTitleAsync(string title);
        Task<IEnumerable<MovieDTO>> SearchByDirectorAsync(string director);
        Task<IEnumerable<MovieDTO>> SearchByGenreAsync(string genre);
    }
}
