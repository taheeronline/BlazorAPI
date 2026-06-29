using System.Net.Http.Json;
using BlazorAPI.BlazorUI.DTOs.UserDTOs;
using BlazorAPI.BlazorUI.Services.Interface;
using BlazorAPI.BlazorUI.Wrapper;

namespace BlazorAPI.BlazorUI.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<PagedResult<UserDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<PagedResult<UserDTO>>(
                $"api/users?page={page}&pageSize={pageSize}",
                cancellationToken);

            return response ?? new PagedResult<UserDTO>();
        }

        public async Task<UserDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<UserDTO>($"api/users/{id}", cancellationToken);
        }

        public async Task<UserDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _httpClient.GetFromJsonAsync<UserDTO>($"api/users/search/email/{Uri.EscapeDataString(email)}", cancellationToken);
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO createDTO, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users", createDTO, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var createdUser = await response.Content.ReadFromJsonAsync<UserDTO>(cancellationToken: cancellationToken);
                return createdUser ?? new UserDTO();
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO updateDTO, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", updateDTO, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    return await response.Content.ReadFromJsonAsync<UserDTO>(cancellationToken: cancellationToken) ?? new UserDTO();
                }
                return new UserDTO();
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }

        public async Task DeleteUserAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync($"api/users/{id}?modifiedById={modifiedById}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
            }
        }

        public async Task<UserDTO?> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", loginDTO, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDTO>(cancellationToken: cancellationToken);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Expected response for bad passwords/emails, returning null so UI handles the error
                return null;
            }

            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"API Error {response.StatusCode}: {errorMessage}");
        }
    }
}