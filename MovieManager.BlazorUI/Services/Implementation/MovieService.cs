using MovieManager.BlazorUI.DTOs.MovieDTOs;
using MovieManager.BlazorUI.Services.Interface;

namespace MovieManager.BlazorUI.Services.Implementation
{
    public class MovieService : iMovieService
    {
        private readonly HttpClient _httpClient;

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<MovieDTO>> GetAll()
        {
            var movies = await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>("api/movies");

            // Returns the movies if not null, otherwise returns an empty collection
            return movies ?? Enumerable.Empty<MovieDTO>();
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

        public async Task<IEnumerable<MovieDTO>> GetByDirector(string director)
        {
            // Updated to match [HttpGet("search/director/{director}")]
            return await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>($"api/movies/search/director/{Uri.EscapeDataString(director)}")
                   ?? Enumerable.Empty<MovieDTO>();
        }

        public async Task<IEnumerable<MovieDTO>> GetByGenre(string genre)
        {
            // Updated to match [HttpGet("search/genre/{genre}")]
            return await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>($"api/movies/search/genre/{Uri.EscapeDataString(genre)}")
                   ?? Enumerable.Empty<MovieDTO>();
        }

        public async Task<MovieDTO?> GetById(int id)
        {
            // This route matches [HttpGet("{id}")] exactly as before, 
            // but using async/await and returning nullable (?) is safer.
            var movie = await _httpClient.GetFromJsonAsync<MovieDTO>($"api/movies/{id}");
            return movie ?? new MovieDTO();
        }

        public async Task<IEnumerable<MovieDTO>> GetByTitle(string title)
        {
            // Updated to match [HttpGet("search/title/{title}")]
            // NOTE: Return type changed from MovieDTO to IEnumerable<MovieDTO>
            return await _httpClient.GetFromJsonAsync<IEnumerable<MovieDTO>>($"api/movies/search/title/{Uri.EscapeDataString(title)}")
                   ?? Enumerable.Empty<MovieDTO>();
        }
    }
}
