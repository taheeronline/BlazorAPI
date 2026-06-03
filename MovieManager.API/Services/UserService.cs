using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using MovieManager.API.Models;
using MovieManager.API.Persistence;

namespace MovieManager.API.Services
{
    public class UserService : IUserService
    {
        private readonly MovieDbContext _db;
        private readonly ILogger<UserService> _logger;

        public UserService(MovieDbContext db, ILogger<UserService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ValidatePasswordAsync(User user, string password)
        {
            if (user == null) return false;
            return VerifyHash(password, user.PasswordHash);
        }

        public async Task<User> CreateUserAsync(string username, string email, string password, string role = "User")
        {
            var user = new User
            {
                Username = username,
                Email = email,
                Role = role,
                PasswordHash = HashPassword(password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task RecordLoginAsync(User user, string? ipAddress = null, string? userAgent = null)
        {
            user.LastLogin = DateTimeOffset.UtcNow;
            _db.Users.Update(user);

            var log = new LoginLog
            {
                UserId = user.Id,
                LoginTime = DateTimeOffset.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _db.LoginLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        // Simple PBKDF2 password hashing
        private static string HashPassword(string password)
        {
            var saltSize = 16;
            var iterations = 10000;
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[saltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var result = new byte[saltSize + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, saltSize);
            Buffer.BlockCopy(hash, 0, result, saltSize, hash.Length);

            return Convert.ToBase64String(result);
        }

        private static bool VerifyHash(string password, string storedHash)
        {
            try
            {
                var bytes = Convert.FromBase64String(storedHash);
                var saltSize = 16;
                var salt = new byte[saltSize];
                Buffer.BlockCopy(bytes, 0, salt, 0, saltSize);

                var hash = new byte[32];
                Buffer.BlockCopy(bytes, saltSize, hash, 0, 32);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                var newHash = pbkdf2.GetBytes(32);

                return CryptographicOperations.FixedTimeEquals(newHash, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
