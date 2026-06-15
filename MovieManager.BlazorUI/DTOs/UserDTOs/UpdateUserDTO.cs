using System.ComponentModel.DataAnnotations;

namespace MovieManager.BlazorUI.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = string.Empty;
    }
}
