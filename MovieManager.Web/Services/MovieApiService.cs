using MovieManager.Web.DTOs;

namespace MovieManager.Web.Services
{
    public class MovieApiService : iMovieApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MovieApiService> _logger;
        private const string BaseRoute = "api/movies";

        public MovieApiService(HttpClient httpClient, ILogger<MovieApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieDTO>> GetAllMoviesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>(BaseRoute) ?? Array.Empty<MovieDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve movies");
                return Array.Empty<MovieDTO>();
            }
        }

        public async Task<MovieDTO?> GetMovieByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MovieDTO>($"{BaseRoute}/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Movie with ID {Id} not found.", id);
                return null;
            }
        }

        public async Task<MovieDTO?> CreateMovieAsync(CreateMovieDTO movie)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseRoute, movie);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MovieDTO>();
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to create movie. Status: {Status}, Error: {Error}", response.StatusCode, error);
            return null;
        }

        public async Task<MovieDTO?> UpdateMovieAsync(Guid id, UpdateMovieDTO movie)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseRoute}/{id}", movie);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MovieDTO>();
            }

            _logger.LogError("Failed to update movie {Id}. Status: {Status}", id, response.StatusCode);
            return null;
        }

        public async Task<bool> DeleteMovieAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseRoute}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<MovieDTO>> SearchByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return await GetAllMoviesAsync();
            return await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>($"{BaseRoute}/search/title/{Uri.EscapeDataString(title)}") ?? Array.Empty<MovieDTO>();
        }

        public async Task<IEnumerable<MovieDTO>> SearchByDirectorAsync(string director)
        {
            if (string.IsNullOrWhiteSpace(director)) return await GetAllMoviesAsync();
            return await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>($"{BaseRoute}/search/director/{Uri.EscapeDataString(director)}") ?? Array.Empty<MovieDTO>();
        }

        public async Task<IEnumerable<MovieDTO>> SearchByGenreAsync(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre)) return await GetAllMoviesAsync();
            return await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>($"{BaseRoute}/search/genre/{Uri.EscapeDataString(genre)}") ?? Array.Empty<MovieDTO>();
        }
    }
}
