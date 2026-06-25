using BlazorAPI.BlazorUI.DTOs.MovieDTOs;
using BlazorAPI.BlazorUI.Services.Interface;
using BlazorAPI.BlazorUI.Wrapper;

namespace BlazorAPI.BlazorUI.Services.Implementation
{
    public class MovieService : iMovieService
    {
        private readonly HttpClient _httpClient;

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<PagedResult<MovieDTO>> GetAll(int page, int pageSize)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>($"api/movies?page={page}&pageSize={pageSize}");
            return response ?? new PagedResult<MovieDTO>();
        }

        public async Task<MovieDTO> CreateMovie(CreateMovieDTO createMovieDTO)
        {
            // 1. Send the POST request to your API endpoint
            var response = await _httpClient.PostAsJsonAsync("api/movies", createMovieDTO);

            // 2. Check if the API returned a 2xx success status code
            if (response.IsSuccessStatusCode)
            {
                // Optional: If your API returns the newly created movie (with its generated Database ID)
                // you can deserialize it and return it back to the Blazor component.
                var createdMovie = await response.Content.ReadFromJsonAsync<MovieDTO>();
                return createdMovie ?? new MovieDTO();
            }
            else
            {
                // 3. Handle errors (e.g., validation errors from the API)
                var errorMessage = await response.Content.ReadAsStringAsync();

                // You can throw an exception, log the error, or return null based on your architecture.
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task<MovieDTO> UpdateMovie(int id, UpdateMovieDTO updateMovieDTO)
        {
            // Send a PUT request to the API with the DTO serialized as JSON
            var response = await _httpClient.PutAsJsonAsync($"api/movies/{id}", updateMovieDTO);

            if (response.IsSuccessStatusCode)
            {
                // APIs typically return 204 No Content for PUT, but if yours returns the updated object (200 OK):
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    return await response.Content.ReadFromJsonAsync<MovieDTO>()?? new MovieDTO();
                }

                return new MovieDTO(); // Update succeeded, but no data returned
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public Task DeleteMovie(int id, int modifiedById)
        {
            // Send a DELETE request with the modifiedById as a query parameter
            return _httpClient.DeleteAsync($"api/movies/{id}?modifiedById={modifiedById}");
        }
        public async Task<PagedResult<MovieDTO>> GetByTitle(string title, int page = 1, int pageSize = 10)
        {
            // Appends page and pageSize to the query string
            return await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>(
                $"api/movies/search/title/{Uri.EscapeDataString(title)}?page={page}&pageSize={pageSize}")
                   ?? new PagedResult<MovieDTO>();
        }

        public async Task<PagedResult<MovieDTO>> GetByDirector(string director, int page = 1, int pageSize = 10)
        {
            return await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>(
                $"api/movies/search/director/{Uri.EscapeDataString(director)}?page={page}&pageSize={pageSize}")
                   ?? new PagedResult<MovieDTO>();
        }

        public async Task<PagedResult<MovieDTO>> GetByGenre(string genre, int page = 1, int pageSize = 10)
        {
            return await _httpClient.GetFromJsonAsync<PagedResult<MovieDTO>>(
                $"api/movies/search/genre/{Uri.EscapeDataString(genre)}?page={page}&pageSize={pageSize}")
                   ?? new PagedResult<MovieDTO>();
        }
        public async Task<MovieDTO?> GetById(int id)
        {
            // This route matches [HttpGet("{id}")] exactly as before, 
            // but using async/await and returning nullable (?) is safer.
            var movie = await _httpClient.GetFromJsonAsync<MovieDTO>($"api/movies/{id}");
            return movie ?? new MovieDTO();
        }

    }
}
