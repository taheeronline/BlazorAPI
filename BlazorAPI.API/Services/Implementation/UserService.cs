using BlazorAPI.API.DTOs.UserDTOs;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Models;
using BlazorAPI.API.Persistence;
using BlazorAPI.API.Services.Interface;
using BlazorAPI.API.Wrapper;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BlazorAPI.API.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(MyDbContext context, ILogger<UserService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedResult<UserDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                var baseQuery = _context.Users
                    .AsNoTracking()
                    .Where(u => !(u.IsDeleted ?? false));

                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var items = await baseQuery
                    .OrderBy(u => u.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => MapToDto(u))
                    .ToListAsync(cancellationToken);

                return new PagedResult<UserDTO>
                {
                    Items = items,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users for page {Page}.", page);
                throw new UserException("Failed to retrieve users.", 500);
            }
        }

        public async Task<UserDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "User");

            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id && !(u.IsDeleted ?? false), cancellationToken);

                return user == null ? null : MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}.", id);
                throw new UserException("Failed to retrieve user.", 500);
            }
        }

        public async Task<UserDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new UserValidationException("Email cannot be empty.");

            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !(u.IsDeleted ?? false), cancellationToken);

                return user == null ? null : MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email.");
                throw new UserException("Failed to retrieve user.", 500);
            }
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO createDTO, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(createDTO);

            try
            {
                if (await _context.Users.AnyAsync(u => (u.Email == createDTO.Email || u.UserName == createDTO.UserName) && !(u.IsDeleted ?? false), cancellationToken))
                {
                    throw new UserValidationException("A user with this email or username already exists.");
                }

                string hashedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(createDTO.Password));

                var user = User.Create(
                    createDTO.Name,
                    createDTO.UserName,
                    createDTO.Email,
                    createDTO.Password,
                    hashedPassword,
                    createDTO.Role,
                    createDTO.CreatedBy
                );

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User created with ID {UserId}.", user.Id);
                return MapToDto(user);
            }
            catch (UserValidationException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                throw new UserException("Failed to create user.", 500);
            }
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO updateDTO, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "User");
            ArgumentNullException.ThrowIfNull(updateDTO);

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id && !(u.IsDeleted ?? false), cancellationToken);

                if (user == null) throw new UserNotFoundException(id);

                user.Update(
                    updateDTO.Name,
                    updateDTO.UserName,
                    updateDTO.Email,
                    updateDTO.Role,
                    updateDTO.ModifiedBy
                );

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User with ID {UserId} updated.", id);
                return MapToDto(user);
            }
            catch (UserNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}.", id);
                throw new UserException("Failed to update user.", 500);
            }
        }

        public async Task DeleteUserAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "User");
            GuardPositiveId(modifiedById, "ModifiedBy");

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id && !(u.IsDeleted ?? false), cancellationToken);

                if (user == null) throw new UserNotFoundException(id);

                user.MarkAsDeleted(modifiedById);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User with ID {UserId} soft-deleted by user {ModifiedById}.", id, modifiedById);
            }
            catch (UserNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}.", id);
                throw new UserException("Failed to delete user.", 500);
            }
        }

        public async Task<UserDTO?> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(loginDTO);

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDTO.UserName.ToLower() && !(u.IsDeleted ?? false), cancellationToken);

                if (user == null) return null;

                string hashedInput = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(loginDTO.Password));

                if (user.HashPassword == hashedInput)
                {
                    return MapToDto(user);
                }

                _logger.LogWarning("Failed login attempt for username {UserName}.", loginDTO.UserName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username {UserName}.", loginDTO.UserName);
                throw new UserException("Failed to process login.", 500);
            }
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private static UserDTO MapToDto(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role
        };

        private static void GuardPositiveId(int id, string paramName)
        {
            if (id <= 0) throw new UserValidationException($"{paramName} ID must be greater than zero.");
        }
    }
}