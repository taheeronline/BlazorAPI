using Microsoft.EntityFrameworkCore;
using MovieManager.API.DTOs.UserDTOs;
using MovieManager.API.Models;
using MovieManager.API.Persistence;

namespace MovieManager.API.Services
{
    public class UserService : iUserService
    {
        private readonly MovieDbContext _context;

        public UserService(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            var users = await _context.Users.ToListAsync();

            // Map the entity list to a DTO list
            return users.Select(MapToResponseDTO);
        }

        public async Task<UserDTO?> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user == null ? null : MapToResponseDTO(user);
        }

        public async Task<UserDTO?> GetByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            return user == null ? null : MapToResponseDTO(user);
        }

        public async Task<UserDTO> CreateUser(CreateUserDTO createDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Email == createDTO.Email || u.UserName == createDTO.UserName))
            {
                throw new InvalidOperationException("A user with this email or username already exists.");
            }

            string hashedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(createDTO.Password));

            var user = User.Create(
                createDTO.Name,
                createDTO.UserName,
                createDTO.Email,
                createDTO.Password,
                hashedPassword,
                createDTO.Role,
                createDTO.CreatedBy);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToResponseDTO(user);
        }

        public async Task UpdateUser(int id, UpdateUserDTO updateDTO)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            user.Update(updateDTO.Name, updateDTO.UserName, updateDTO.Email, updateDTO.Role, updateDTO.ModifiedBy);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int id, int modifiedById)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            user.MarkAsDeleted(modifiedById);
            await _context.SaveChangesAsync();
        }

        public async Task<UserDTO?> Login(LoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDto.UserName.ToLower());

            if (user == null) return null;

            string hashedInput = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));

            if (user.HashPassword == hashedInput)
            {
                return MapToResponseDTO(user);
            }

            return null;
        }

        // --- Helper Method for cleaner mapping ---
        private static UserDTO MapToResponseDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}