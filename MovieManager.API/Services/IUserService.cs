using MovieManager.API.Models;

namespace MovieManager.API.Services
{
    public interface IUserService
    {
        Task<User?> FindByUsernameAsync(string username);
        Task<bool> ValidatePasswordAsync(User user, string password);
        Task<User> CreateUserAsync(string username, string email, string password, string role = "User");
        Task RecordLoginAsync(User user, string? ipAddress = null, string? userAgent = null);
    }
}
