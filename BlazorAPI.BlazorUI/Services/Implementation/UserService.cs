using BlazorAPI.BlazorUI.DTOs.UserDTOs;
using BlazorAPI.BlazorUI.Services.Interface;

namespace BlazorAPI.BlazorUI.Services.Implementation
{
    public class UserService : iUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>("api/users")
                   ?? Enumerable.Empty<UserDTO>();
        }

        public async Task<UserDTO?> GetById(int id)
        {
            return await _httpClient.GetFromJsonAsync<UserDTO>($"api/users/{id}");
        }

        public async Task<UserDTO?> GetByEmail(string email)
        {
            // Escaping the email in case it contains special characters
            return await _httpClient.GetFromJsonAsync<UserDTO>($"api/users/search/email/{Uri.EscapeDataString(email)}");
        }

        public async Task<UserDTO> CreateUser(CreateUserDTO createDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users", createDTO);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDTO>();
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task UpdateUser(int id, UpdateUserDTO updateDTO)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", updateDTO);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task DeleteUser(int id, int modifiedById)
        {
            var response = await _httpClient.DeleteAsync($"api/users/{id}?modifiedById={modifiedById}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task<UserDTO?> Login(LoginDTO loginDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", loginDTO);

            if (response.IsSuccessStatusCode)
            {
                // Login successful, return the user details
                return await response.Content.ReadFromJsonAsync<UserDTO>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Expected response for bad passwords/emails, return null so the UI can show an error
                return null;
            }
            else
            {
                // Unexpected server errors (500) or bad requests (400)
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }
    }
}