using System.ComponentModel.DataAnnotations;

namespace MovieManager.API.DTOs.UserDTOs
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        // Optionally default to "User" to prevent accidental Admin creation
        public string Role { get; set; } = "User";

        public int CreatedBy { get; set; }

    }
}
