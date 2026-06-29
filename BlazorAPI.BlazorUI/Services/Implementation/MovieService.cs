using System.Net.Http.Json;
using BlazorAPI.BlazorUI.DTOs.MovieDTOs;
using BlazorAPI.BlazorUI.Services.Interface;
using BlazorAPI.BlazorUI.Wrapper;

namespace BlazorAPI.BlazorUI.Services.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<PagedResult<MovieDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>(
                $"api/movies?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<MovieDTO>();
        }

        public async Task<MovieDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var movie = await _httpClient.GetFromJsonAsync<MovieDTO>($"api/movies/{id}", cancellationToken);
            return movie ?? new MovieDTO();
        }

        public async Task<MovieDTO> CreateMovieAsync(CreateMovieDTO createMovieDTO, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/movies", createMovieDTO, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var createdMovie = await response.Content.ReadFromJsonAsync<MovieDTO>(cancellationToken: cancellationToken);
                return createdMovie ?? new MovieDTO();
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }

        public async Task<MovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO updateMovieDTO, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/movies/{id}", updateMovieDTO, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    return await response.Content.ReadFromJsonAsync<MovieDTO>(cancellationToken: cancellationToken) ?? new MovieDTO();
                }
                return new MovieDTO();
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }

        public async Task DeleteMovieAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/movies/{id}?modifiedById={modifiedById}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task<PagedResult<MovieDTO>> GetByTitleAsync(string title, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>(
                $"api/movies/search/title/{Uri.EscapeDataString(title)}?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<MovieDTO>();
        }

        public async Task<PagedResult<MovieDTO>> GetByDirectorAsync(string director, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>(
                $"api/movies/search/director/{Uri.EscapeDataString(director)}?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<MovieDTO>();
        }

        public async Task<PagedResult<MovieDTO>> GetByGenreAsync(string genre, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>(
                $"api/movies/search/genre/{Uri.EscapeDataString(genre)}?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<MovieDTO>();
        }
    }
}